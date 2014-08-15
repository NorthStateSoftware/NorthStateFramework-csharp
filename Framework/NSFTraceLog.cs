// Copyright 2004-2014, North State Software, LLC.  All rights reserved.

// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;

using NSFString = System.String;

namespace NorthStateSoftware.NorthStateFramework
{
    /// <summary>
    /// Represents a trace log.
    /// </summary>
    public class NSFTraceLog : NSFTaggedObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates a trace log
        /// </summary>
        /// <param name="name">The name of the log.</param>
        /// <remarks>
        /// By default, thread priority is set to lowest.
        /// </remarks>
        public NSFTraceLog(NSFString name)
            : base(name)
        {
            eventHandler = new NSFEventHandler(Name, new NSFEventThread(Name, NSFOSThread.LowestPriority));
            eventHandler.LoggingEnabled = false;

            traceAddEvent = new NSFDataEvent<NSFXMLElement>(TraceAddString, eventHandler);
            traceSaveEvent = new NSFDataEvent<NSFString>(TraceSaveString, eventHandler);

            eventHandler.addEventReaction(traceAddEvent, addTraceToLog);
            eventHandler.addEventReaction(traceSaveEvent, saveTrace);

            eventHandler.startEventHandler();

            addTrace(NSFTraceTags.InformationalTag, NSFTraceTags.NameTag, "TraceStart");
        }

        #endregion Public Constructors

        #region Public Fields, Events, and Properties

        /// <summary>
        /// Gets the framework trace log.
        /// </summary>
        /// <remarks>
        /// This log is where framework traces are recorded, and where static logging operations record traces.
        /// Most application use this log for custom trace recording, as well.
        /// </remarks>
        public static NSFTraceLog PrimaryTraceLog
        {
            get
            {
                return primaryTraceLog;
            }
        }

        /// <summary>
        /// Gets or sets the flag indicating if tracing is enabled.
        /// </summary>
        /// <remarks>
        /// If tracing is disabled, no additions will be made to the trace log.
        /// </remarks>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of trace entries in the log.
        /// </summary>
        /// <remarks>
        /// Once the trace limit is reached, old traces are deleted in favor of new ones.
        /// </remarks>
        public UInt32 MaxTraces
        {
            get { return maxTraces; }
            set { maxTraces = value; }
        }

        #endregion Public Fields, Events, and Properties

        #region Public Methods

        /// <summary>
        /// Adds a trace to the log.
        /// </summary>
        /// <param name="type">The type of the trace.</param>
        /// <param name="tag">The tag associated with the trace.</param>
        /// <param name="data">The data associated with the tag.</param>
        public void addTrace(NSFString type, NSFString tag, NSFString data)
        {
            addTrace(type, tag, data, null, null, null, null);
        }

        /// <summary>
        /// Adds a trace to the log.
        /// </summary>
        /// <param name="type">The type of the trace.</param>
        /// <param name="tag1">The first tag associated with the trace.</param>
        /// <param name="data1">The data associated with the first tag.</param>
        /// <param name="tag2">The second tag associated with the trace.</param>
        /// <param name="data2">The data associated with the second tag.</param>
        public void addTrace(NSFString type, NSFString tag1, NSFString data1, NSFString tag2, NSFString data2)
        {
            addTrace(type, tag1, data1, tag2, data2, null, null);
        }

        /// <summary>
        /// Adds a trace to the log.
        /// </summary>
        /// <param name="type">The type of the trace.</param>
        /// <param name="tag1">The first tag associated with the trace.</param>
        /// <param name="data1">The data associated with the first tag.</param>
        /// <param name="tag2">The second tag associated with the trace.</param>
        /// <param name="data2">The data associated with the second tag.</param>
        /// <param name="tag3">The third tag associated with the trace.</param>
        /// <param name="data3">The data associated with the third tag.</param>
        public void addTrace(NSFString type, NSFString tag1, NSFString data1, NSFString tag2, NSFString data2, NSFString tag3, NSFString data3)
        {
            // If logging is not enabled then return without action
            if (!enabled)
            {
                return;
            }

            // Lock to prevent events from being added to the queue with timestamps out of order
            lock (traceLogMutex)
            {
                try
                {
                    // Create the new trace element
                    NSFXMLElement trace = new NSFXMLElement(TraceTag);

                    // Add the time
                    trace.addChildElementBack(new NSFXMLElement(TimeTag, NSFTimerThread.PrimaryTimerThread.CurrentTime.ToString()));

                    // Add a trace for the type
                    NSFXMLElement typeTrace = new NSFXMLElement(type);
                    trace.addChildElementBack(typeTrace);

                    // Add the new data under the type element;
                    typeTrace.addChildElementBack(new NSFXMLElement(tag1, data1));

                    if (tag2 != null) typeTrace.addChildElementBack(new NSFXMLElement(tag2, data2));
                    if (tag3 != null) typeTrace.addChildElementBack(new NSFXMLElement(tag3, data3));

                    eventHandler.queueEvent(traceAddEvent.copy(trace));
                }
                catch
                {
                    // If unable to add a trace, just do nothing, because calling the exception handler may result in an infinite loop
                }
            }
        }

        /// <summary>
        /// Have all traces that have been added been logged
        /// </summary>
        /// <returns>
        /// True if the NSFTraceLog has logged all the added traces.
        /// False if there are traces that have yet to be processed.
        /// </returns>
        public bool allTracesLogged()
        {
            return !eventHandler.hasEvent();
        }

        /// <summary>
        /// Saves the trace log to a file with the name of the log.
        /// </summary>
        /// <remarks>
        /// The trace log is saved in xml format.
        /// </remarks>
        public void saveLog()
        {
            saveLog(Name);
        }

        /// <summary>
        /// Saves the trace log to the specified filename.
        /// </summary>
        /// <param name="fileName">The relative or fully qualified name for the file.</param>
        /// <remarks>
        /// The trace log is saved in xml format.
        /// </remarks>
        public void saveLog(NSFString fileName)
        {
            if (enabled)
            {
                traceSaveEvent.copy(fileName).queueEvent();
            }
        }

        #endregion Public Methods

        #region Protected Fields, Events, and Properties

        #endregion Protected Fields, Events, and Properties

        #region Protected Methods

        /// <summary>
        /// Reaction that does the work adding a trace element to the log.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        protected void addTraceToLog(NSFEventContext context)
        {
            try
            {
                xmlLog.jumpToRootElement();

                int numberOfChildElements;

                while (xmlLog.getNumberOfChildElements(out numberOfChildElements) &&
                       (numberOfChildElements >= maxTraces))
                {
                    xmlLog.deleteChildElementFront();
                }
                xmlLog.addChildElementBack(((NSFDataEvent<NSFXMLElement>)context.Event).Data);
            }
            catch
            {
                // If unable to add a trace, just do nothing, because calling the exception handler may result in an infinite loop
            }
        }

        /// <summary>
        /// Reaction that does the work of saving the file.
        /// </summary>
        /// <param name="context">Additional contextual information.</param>
        protected void saveTrace(NSFEventContext context)
        {
            try
            {
                addTrace(NSFTraceTags.InformationalTag, NSFTraceTags.NameTag, TraceSaveString);
                xmlLog.save(((NSFDataEvent<NSFString>)context.Event).Data);
                addTrace(NSFTraceTags.InformationalTag, NSFTraceTags.NameTag, TraceSaveCompleteString);
            }
            catch (Exception e)
            {
                // If unable to save a trace, just add a trace with the exception message, because calling the exception handler may result in an infinite loop
                addTrace(NSFTraceTags.ExceptionTag, NSFTraceTags.MessageTag, Name + " exception saving trace: " + e.ToString());
            }
        }

        #endregion Protected Methods

        #region Private Fields, Events, and Properties

        private static NSFTraceLog primaryTraceLog = new NSFTraceLog("PrimaryTraceLog");

        private const NSFString TimeTag = "Time";
        private const NSFString TraceAddString = "TraceAdd";
        private const NSFString TraceLogTag = "TraceLog";
        private const NSFString TraceSaveCompleteString = "TraceSaveComplete";
        private const NSFString TraceSaveString = "TraceSave";
        private const NSFString TraceTag = "Trace";

        private bool enabled = true;
        private NSFEventHandler eventHandler;
        private UInt32 maxTraces = 5000;
        private object traceLogMutex = new object();
        private NSFXMLDocument xmlLog = new NSFXMLDocument(TraceLogTag);

        private NSFDataEvent<NSFXMLElement> traceAddEvent;
        private NSFDataEvent<NSFString> traceSaveEvent;

        #endregion Private Fields, Events, and Properties
    }

    /// <summary>
    /// Represents commonly used tags for the trace log.
    /// </summary>
    public class NSFTraceTags
    {
        #region Public Fields, Events, and Properties

        public const NSFString ActionExecutedTag = "ActionExecuted";
        public const NSFString ActionTag = "Action";
        public const NSFString DestinationTag = "Destination";
        public const NSFString ErrorTag = "Error";
        public const NSFString EventQueuedTag = "EventQueued";
        public const NSFString EventTag = "Event";
        public const NSFString ExceptionTag = "Exception";
        public const NSFString InformationalTag = "Informational";
        public const NSFString MessageReceivedTag = "MessageReceived";
        public const NSFString MessageSentTag = "MessageSent";
        public const NSFString MessageTag = "Message";
        public const NSFString NameTag = "Name";
        public const NSFString ObjectTag = "Object";
        public const NSFString SourceTag = "Source";
        public const NSFString StateEnteredTag = "StateEntered";
        public const NSFString StateMachineTag = "StateMachine";
        public const NSFString StateTag = "State";
        public const NSFString UnknownTag = "Unknown";
        public const NSFString ValueTag = "Value";
        public const NSFString VariableTag = "Variable";

        #endregion Public Fields, Events, and Properties
    }
}