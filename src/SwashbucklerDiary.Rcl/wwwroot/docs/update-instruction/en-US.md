# Update Announcement

[toc]

## Update Time

2025.12.06

## Preface

Special thanks to the patrons and encouragers `烽火入眉弯`, `AgSnP`, `无名侠客`, `*.`, `*G`, `x*x`, `*班`, and others.

The project's development is inseparable from everyone's support. Friends who are interested can support it [Sponsor](sponsor).

This release still focuses on repair and optimizations, and the underlying framework has been updated to the latest and fastest .NET 10.

## New Features

* Link Cards (Renders links in a card format) [Jump to related settings](diarySetting)
> Regarding link cards, websites supporting the Open Graph protocol provide the best experience. Websites with enabled verification (like Zhihu, Douyin) are not supported. [Click to view examples](#Link Card Examples)
* Default title for diary cards [Jump to related settings](diaryCardSetting)

## Fixes

* App lock could not be enabled.
* App lock page would display pop-ups from other pages.
* Markdown rendering in diary cards: image links containing spaces would not display.
* Outline settings were not displayed in mobile device settings.
* Text not wrapping in some locations.
* Album art for some songs not displaying on some platforms.
* Exception occurred when changing the theme during backup or import/restore processes.
* Quick record not taking effect.
* Data not updating immediately after restoring a backup.
* Accessing privacy mode database via specific links.
* Anchor links containing Chinese characters in the path not jumping.
* App features search does not work.
* Android client app interface would refresh after changing the system font size.
* macOS client unable to add audio files.
* Persistent failure on Web.

## Optimizations

* Timing of the fingerprint lock verification pop-up.
* Update pop-up now displays more content.
* Excluding invalid paths when fetching media file paths.
* Adjusted theme colors.
* WebDAV download page styling.
* Speed of generating images for sharing.
* Experience on low-performance Android devices.

## Link Card Examples

https://b23.tv/lU7tXTW
https://www.xiaohongshu.com/discovery/item/68e37c6e000000000703b135?xsec_token=ABJPCNguHxWYsWqrsux2KS6KFkgXll7W5SEEnA6tcJdag=
https://163cn.tv/V1M0rnS
https://mp.weixin.qq.com/s/md5avqAvjFfR4c9knfWAOg
https://v.qq.com/x/cover/mcv8hkc8zk8lnov/x0036x5qqsr.html