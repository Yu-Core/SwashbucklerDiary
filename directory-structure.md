```
SwashbucklerDiary/
├── .editorconfig                # 编辑器/IDE 配置，用于统一代码风格 (缩进/编码/换行等)
├── .gitattributes               # Git 属性文件 (定义 git 行为，如换行、差异显示等)
├── .gitignore                   # Git 忽略规则 —— 指定不纳入版本控制的文件/目录
├── .github/                     # GitHub 相关配置（CI／Release／Issue 模板等）
├── res/                         # 项目资源 (resource) —— 比如截图、图片等静态资源
├── src/
│   ├── Maui.Essentials              # 移植 MAUI 上的一些功能
│   ├── SwashbucklerDiary.Gtk/       # 基于Gtk、WebKitGtk和Blazor Hybrid的Linux客户端实现   
│   ├── SwashbucklerDiary.Maui/      # 基于.NET MAUI和Blazor Hybrid的 跨平台的客户端实现（Android、iOS、macOS 和 Windows）
│   │   ├── Extensions/ServiceCollectionExtensions/  # 扩展方法 (如日志系统注册等)  
│   │   ├── 其它平台相关代码/资源                     # 各平台 UI 启动、兼容层、资源文件等  
│   │   └── MauiProgram.cs, App.xaml, MainPage.xaml  # MAUI 启动与主界面定义   
│   ├── SwashbucklerDiary.Rcl/       # 共享 Razor 组件库 (RCL — Razor Class Library)，定义不同项目共享的UI组件和页面视图
│   │   ├── Layout/                  # 主界面布局相关组件 (导航、底栏等)
│   │   ├── Pages/                   # 各页面 (主页、日记页、标签页等)
│   │   ├── Components/              # 可复用 UI 组件 (按钮、对话框、卡片、列表等)
│   │   ├── Services/                # 业务服务 (状态管理、应用逻辑、数据服务等)
│   │   ├── Models/                  # 数据模型 (日记条目、标签、元数据等结构定义)
│   │   ├── Repository/              # 数据访问 / 仓储 (存取日记内容、日志、元数据等)  
│   │   └── wwwroot/                 # Web 静态资源 (本地化语言包、图标、样式、脚本等)
│   ├── SwashbucklerDiary.Rcl.Hybrid/   # RCL 的客户端专用实现，继承并扩展Rcl中的基础组件和服务接口
│   ├── SwashbucklerDiary.Rcl.Web/   # RCL 的浏览器端专用实现，继承并扩展Rcl中的基础组件和服务接口
│   ├── SwashbucklerDiary.Server/       # 基于 Blazor Server 的 Web 版本实现 (浏览器端，服务器渲染) 
|   ├── SwashbucklerDiary.Shared/       # 核心数据模型与共享基础设施，扩展方法、常量、枚举等
│   └── SwashbucklerDiary.WebAssembly/  # 基于 Blazor WebAssembly 的 Web 版本实现 (浏览器端，纯前端) 
├── LICENSE.txt                  # 开源许可证 (本项目采用 AGPL-3.0)
├── README.md                    # 中文说明／项目介绍／使用方法／功能特性等
├── README.en-US.md              # 英文版本的项目说明 (方便国际用户)
├── SwashbucklerDiary.Gtk.sln    # 用于 GTK Linux 桌面 (Blazor Hybrid) 平台构建的解决方案 (适合 Linux)
├── SwashbucklerDiary.Maui.sln   # 用于 .NET MAUI (Blazor Hybrid) 构建 (移动 / 跨平台 GUI) 的解决方案 (适合移动 / 桌面)
├── SwashbucklerDiary.Server.sln  # 用于构建 Blazor Server 版本的解决方案 (Web 平台)
├── SwashbucklerDiary.sln        # Visual Studio / .NET 的主解决方案文件 —— 管理整个项目结构／依赖／构建配置
├── SwashbucklerDiary.WebAssembly.sln  # 用于构建 Blazor WebAssembly 版本的解决方案 (Web 平台)
├── download-guide.md            # (可选) 下载 / 安装 / 发布 相关指南或说明文档
└── …                            # 其他辅助文件 (若有)／根据不同分支可能略有不同
```