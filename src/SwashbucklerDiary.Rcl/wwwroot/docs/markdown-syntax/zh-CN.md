# Markdown 语法速查表

感谢访问 [The Markdown Guide](https://www.markdownguide.org)!

此 Markdown 语法速查表提供了所有 Markdown 语法元素的快速参考。但是此速查表无法涵盖所有极限用法，因此，如果您需要某些语法元素的详细信息，请参阅我们的 [基本语法](https://www.markdownguide.org/basic-syntax) 和 [扩展语法](https://www.markdownguide.org/extended-syntax) 手册。

## 基本语法

这些是 John Gruber 的原始设计文档中列出的元素。所有 Markdown 应用程序都支持这些元素。

### 标题

# H1
## H2
### H3

```
# H1
## H2
### H3
```

### 粗体

**bold text**

```
**bold text**
```

### 斜体

*italicized text*

```
*italicized text*
```

### 引用块

> blockquote

```
> blockquote
```

### 有序列表

1. First item
2. Second item
3. Third item

```
1. First item
2. Second item
3. Third item
```

### 无序列表

- First item
- Second item
- Third item

```
- First item
- Second item
- Third item
```

### 代码

`code`

```
`code`
```

### 分割线

---

```
---
```

### 链接

[Markdown Guide](https://www.markdownguide.org)

```
[Markdown Guide](https://www.markdownguide.org)
```

### 图片

![alt text](https://www.markdownguide.org/assets/images/tux.png)

```
![alt text](https://www.markdownguide.org/assets/images/tux.png)
```

## 扩展语法 (本应用目前仅支持部分)

这些元素通过添加额外的功能扩展了基本语法。但是，并非所有 Markdown 应用程序都支持这些元素。

### 表格

| Syntax | Description |
| ----------- | ----------- |
| Header | Title |
| Paragraph | Text |

```
| Syntax | Description |
| ----------- | ----------- |
| Header | Title |
| Paragraph | Text |
```

### 代码块

```
{
  "firstName": "John",
  "lastName": "Smith",
  "age": 25
}
```

``````
```
{
  "firstName": "John",
  "lastName": "Smith",
  "age": 25
}
```
``````

### 脚注

Here's a sentence with a footnote. [^1]

[^1]: This is the footnote.

```
Here's a sentence with a footnote. [^1]

[^1]: This is the footnote.
```

### 标题编号

### My Great Heading {#custom-id}

```
### My Great Heading {#custom-id}
```

### 定义列表

term
: definition

```
term
: definition
```

### 删除线

~~The world is flat.~~

```
~~The world is flat.~~
```

### 任务列表

- [x] Write the press release
- [ ] Update the website
- [ ] Contact the media

```
- [x] Write the press release
- [ ] Update the website
- [ ] Contact the media
```

### 表情

That is so funny! :joy:

(See also [Copying and Pasting Emoji](https://www.markdownguide.org/extended-syntax/#copying-and-pasting-emoji))

```
That is so funny! :joy:

(See also [Copying and Pasting Emoji](https://www.markdownguide.org/extended-syntax/#copying-and-pasting-emoji))
```

### 高亮

I need to highlight these ==very important words==.

```
I need to highlight these ==very important words==.
```

### 下标

H~2~O

```
H~2~O
```

### 上标

X^2^

```
X^2^
```