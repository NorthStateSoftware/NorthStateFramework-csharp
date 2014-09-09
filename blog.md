---
layout: page
title: Blog
permalink: /Blog/
comments: true
---

<ul class="posts">
	{% for post in site.posts %}
	<li>
		<h2>
		<a href="{{ post.url | prepend: site.baseurl }}">{{ post.date | date: "%b %-d, %Y" }} "{{ post.title }}"</a>
		</h2>
		<article class="post-content">
			{{ post.excerpt }}
		</article>
	</li>
	{% endfor %}
</ul>
