# 更新公告

本期重点：沉浸式状态栏/导航栏（其实准确来说，应该叫透明状态栏/导航栏），基本上没问题，不过不排除在少数设备上会有兼容性问题。如有问题，请尽快反馈。

更新了隐私政策，请到 [隐私政策](https://gitee.com/Yu-core/SwashbucklerDiary/raw/master/src/SwashbucklerDiary.Rcl/wwwroot/docs/privacy-policy/zh-CN.md) 或APP内 关于/隐私政策 查看（用的通用的模板，某家应用市场说与实际不符，就把有问题的那几条去掉了）

## 新增

* 移动端透明状态栏和导航栏（沉浸式）
* web端在移动设备上，深色模式支持改变状态栏（需要浏览器支持，安卓端Chrome和iOS端safari是支持的，edge不支持，一些使用webview的浏览器也不支持）

## 修复

* 用户协议和隐私策略URL地址不正确
* 移动端阅读日记页面的标签，长按会出现阴影
* Windows端不能选择.webp和.jfif等格式的图片
* web端本地媒体文件无法请求
* web端更新公告语言不正确
* iOS端输入后APP窗口高度不正确

## 优化

* 默认启用编辑时自动保存
* 移除一些不必要的提示
* 默认显示日记卡片上的天气和心情图标
* markdown输入体验