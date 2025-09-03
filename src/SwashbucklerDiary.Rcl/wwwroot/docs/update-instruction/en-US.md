# Update Announcement

[toc]

## Update Time

2025.09.03

Commemorating the 80th Anniversary of the Victory of the Chinese People's War of Resistance Against Japanese Aggression and the World Anti-Fascist War.

## Foreword

Thanks to all users for your support, suggestions, and feedback.

Special thanks to **Kun, Aolegeaoo, Xu Yanxi, Youhe, NOTDEFINED, Yun Yi, Yuan, admin, noemi💫, Chapao Fan, Xianjian Houzeng, Lü Xiaobu, Anonymous User, **Hao, Longde Eryin Zhe, A Yu, Makabaka, Wanderer, Anonymous User**, and other users for your donations and encouragement.

The project's development relies on everyone's support. Interested friends can support us at [Sponsor](sponsor).

The focus of this update is multi-language support, application lock, and fixing the issue where the Linux client couldn't load local audio/video files.

## New Features

* Default Template Setting [Jump](templateSetting)
* Sort by last modified time
* Export resource files [Jump](fileBrowse)
* Added anchors to Heading titles
* Copy diary references with anchors
* Diary card Markdown rendering settings [Jump](diaryCardSetting)
* Insert time format settings on the edit page [Jump](diarySetting)
* Support for more languages: German, Spanish, French, Japanese, Korean, Portuguese, Russian, Vietnamese, Traditional Chinese [Jump](mine)
* Media resource files can be deleted individually [Jump](fileBrowse)
* Application Lock, including Number Lock, Pattern Unlock, and Fingerprint Recognition [Jump](appLockSetting)
* Privacy mode password can be reset
* Packaged Android x64, Linux RPM packages
* Add search filtering criteria, all meet or any of them
* Support superscripts and subscripts
* Setting to use the original file name when adding files [Jump](diarySetting)

## Fixes

* Fixed export failure when filtering by year, month, and day
* Fixed incorrect icons in some places
* Fixed abnormal image sharing when multiple diary pages were open
* Fixed abnormal rendering of the donation list
* Fixed page cache not being cleared when navigating back through multiple pages
* Fixed navigation bar button icons potentially not activating correctly
* Fixed incorrect redirection (closing pop-ups instead) when navigating back through multiple pages
* Fixed inability to save or share internal images
* Fixed the issue where deleting unused files did not take effect
* Fixed failure to restore backups from older versions
* Fixed repeated scrolling up and down when editing task lists on mobile
* Fixed abnormal outline node clicks
* Fixed WebDAV configuration failed
* Fixed Mac client's inability to use LAN transfer
* Fixed copy/cut not working in Markdown editor on Mac client
* Fixed external links not opening in Mac client
* Fixed incorrect Dock name on Mac client
* Fixed abnormal behavior when canceling file save on Linux client
* Fixed Linux client's inability to load local audio/video files
* Fixed window not coming to foreground when activated via shared content or URL scheme on Windows client

## Optimizations

* Exclude the current diary or template when referencing diaries or selecting templates
* Optimized performance on first startup
* Removed rendering for deck links from NetEaseDaShen and iyingdi
* Removed error message when canceling avatar editing
* Optimized LAN transfer performance and notifications
* Increased the date picker range on mobile to 100 years in the past and future
* Made the outline more convenient to use on mobile
* Improved loading speed for videos at specified locations on Android client
* Desktop outline display optimization
* Partial pop-up window height optimization