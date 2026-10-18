namespace SwashbucklerDiary.Rcl.Components
{
    /// <summary>
    /// 三套信笺纸的 SVG 背景（1080×1440）。
    /// 日期/天气由 HTML 层渲染，印章文字由 {0} 占位符注入。
    /// </summary>
    public static class LetterPaperTemplateSvg
    {
        internal const string ZhuLan = """
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1080 1440" preserveAspectRatio="xMidYMid meet">
              <defs>
                <linearGradient id="lp-paper" x1="0" y1="0" x2="1" y2="1">
                  <stop offset="0" stop-color="#f8f2e3"/>
                  <stop offset="0.6" stop-color="#f4ecd9"/>
                  <stop offset="1" stop-color="#efe3c9"/>
                </linearGradient>
                <radialGradient id="lp-vign" cx="0.5" cy="0.44" r="0.78">
                  <stop offset="0.62" stop-color="#8a7a5c" stop-opacity="0"/>
                  <stop offset="1" stop-color="#574b35" stop-opacity="0.11"/>
                </radialGradient>
                <filter id="lp-grain" x="0" y="0" width="100%" height="100%">
                  <feTurbulence type="fractalNoise" baseFrequency="0.9" numOctaves="3" seed="7" stitchTiles="stitch"/>
                  <feColorMatrix type="matrix" values="0 0 0 0 0.44  0 0 0 0 0.40  0 0 0 0 0.32  0 0 0 0.055 0"/>
                </filter>
              </defs>
              <rect width="1080" height="1440" fill="url(#lp-paper)"/>
              <rect width="1080" height="1440" filter="url(#lp-grain)" opacity="0.62"/>
              <rect width="1080" height="1440" fill="url(#lp-vign)"/>
              <rect x="96" y="96" width="888" height="1248" fill="none" stroke="#a93226" stroke-width="2.6"/>
              <rect x="108" y="108" width="864" height="1224" fill="none" stroke="#b03a30" stroke-width="1"/>
              <g stroke="#b03a30" stroke-width="1.1" opacity="0.7">
                <line x1="250.5" y1="108" x2="250.5" y2="1276"/>
                <line x1="347" y1="108" x2="347" y2="1276"/>
                <line x1="443.5" y1="108" x2="443.5" y2="1276"/>
                <line x1="540" y1="108" x2="540" y2="1276"/>
                <line x1="636.5" y1="108" x2="636.5" y2="1276"/>
                <line x1="733" y1="108" x2="733" y2="1276"/>
                <line x1="829.5" y1="108" x2="829.5" y2="1276"/>
              </g>
              <g>
                <rect x="488" y="20" width="64" height="64" rx="6" fill="none" stroke="#a93226" stroke-width="3.2"/>
                <text x="520" y="63" text-anchor="middle" font-family="'Jiangxi Zhuokai','LXGW WenKai','KaiTi','STKaiti',serif" font-size="26" fill="#a93226">{0}</text>
              </g>
              <g fill="#4a4238" opacity="0.92" font-family="'Jiangxi Zhuokai','LXGW WenKai','KaiTi','STKaiti',serif">
                <text x="878" y="150" text-anchor="middle" font-size="30">晴</text>
                <text x="878" y="190" text-anchor="middle" font-size="30">窗</text>
              </g>
            </svg>
            """;

        internal const string SuJian = """
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1080 1440" preserveAspectRatio="xMidYMid meet">
              <defs>
                <linearGradient id="lp-paper" x1="0" y1="0" x2="1" y2="1">
                  <stop offset="0" stop-color="#f8f2e3"/>
                  <stop offset="0.6" stop-color="#f4ecd9"/>
                  <stop offset="1" stop-color="#efe3c9"/>
                </linearGradient>
                <radialGradient id="lp-vign" cx="0.5" cy="0.44" r="0.78">
                  <stop offset="0.62" stop-color="#8a7a5c" stop-opacity="0"/>
                  <stop offset="1" stop-color="#574b35" stop-opacity="0.11"/>
                </radialGradient>
                <filter id="lp-grain" x="0" y="0" width="100%" height="100%">
                  <feTurbulence type="fractalNoise" baseFrequency="0.9" numOctaves="3" seed="23" stitchTiles="stitch"/>
                  <feColorMatrix type="matrix" values="0 0 0 0 0.44  0 0 0 0 0.40  0 0 0 0 0.32  0 0 0 0.055 0"/>
                </filter>
              </defs>
              <rect width="1080" height="1440" fill="url(#lp-paper)"/>
              <rect width="1080" height="1440" filter="url(#lp-grain)" opacity="0.62"/>
              <rect width="1080" height="1440" fill="url(#lp-vign)"/>
              <text x="152" y="158" font-family="'Jiangxi Zhuokai','LXGW WenKai','KaiTi','STKaiti',serif" font-size="40" fill="#433c2f">漫记</text>
              <rect x="254" y="128" width="30" height="30" rx="4" fill="none" stroke="#a93226" stroke-width="2.4"/>
              <text x="269" y="153" text-anchor="middle" font-family="'Jiangxi Zhuokai','LXGW WenKai','KaiTi','STKaiti',serif" font-size="17" fill="#a93226">{0}</text>
              <g stroke="#b0a58c" stroke-width="1" opacity="0.5">
                <line x1="150" y1="230" x2="930" y2="230"/>
                <line x1="150" y1="290" x2="930" y2="290"/>
                <line x1="150" y1="350" x2="930" y2="350"/>
                <line x1="150" y1="410" x2="930" y2="410"/>
                <line x1="150" y1="470" x2="930" y2="470"/>
                <line x1="150" y1="530" x2="930" y2="530"/>
                <line x1="150" y1="590" x2="930" y2="590"/>
                <line x1="150" y1="650" x2="930" y2="650"/>
                <line x1="150" y1="710" x2="930" y2="710"/>
                <line x1="150" y1="770" x2="930" y2="770"/>
                <line x1="150" y1="830" x2="930" y2="830"/>
                <line x1="150" y1="890" x2="930" y2="890"/>
                <line x1="150" y1="950" x2="930" y2="950"/>
                <line x1="150" y1="1010" x2="930" y2="1010"/>
                <line x1="150" y1="1070" x2="930" y2="1070"/>
                <line x1="150" y1="1130" x2="930" y2="1130"/>
              </g>
              <circle cx="806" cy="1300" r="36" fill="#d9cdaa" opacity="0.55"/>
              <path d="M 300 1440 Q 480 1315 660 1440 Z" fill="#7a6f57" opacity="0.10"/>
              <path d="M 470 1440 Q 650 1345 840 1440 Z" fill="#7a6f57" opacity="0.13"/>
              <path d="M 660 1440 Q 810 1375 980 1440 Z" fill="#7a6f57" opacity="0.17"/>
              <g stroke="#5d5444" stroke-width="2" fill="none" opacity="0.4">
                <path d="M 400 1260 q 12 -9 24 0"/>
                <path d="M 446 1250 q 9 -7 18 0"/>
                <path d="M 490 1270 q 8 -6 16 0"/>
              </g>
            </svg>
            """;

        internal const string ZhuYing = """
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1080 1440" preserveAspectRatio="xMidYMid meet">
              <defs>
                <linearGradient id="lp-paper" x1="0" y1="0" x2="1" y2="1">
                  <stop offset="0" stop-color="#f8f2e3"/>
                  <stop offset="0.6" stop-color="#f4ecd9"/>
                  <stop offset="1" stop-color="#efe3c9"/>
                </linearGradient>
                <radialGradient id="lp-vign" cx="0.5" cy="0.44" r="0.78">
                  <stop offset="0.62" stop-color="#8a7a5c" stop-opacity="0"/>
                  <stop offset="1" stop-color="#574b35" stop-opacity="0.11"/>
                </radialGradient>
                <filter id="lp-grain" x="0" y="0" width="100%" height="100%">
                  <feTurbulence type="fractalNoise" baseFrequency="0.9" numOctaves="3" seed="41" stitchTiles="stitch"/>
                  <feColorMatrix type="matrix" values="0 0 0 0 0.44  0 0 0 0 0.40  0 0 0 0 0.32  0 0 0 0.055 0"/>
                </filter>
                <filter id="lp-blot" x="-40%" y="-40%" width="180%" height="180%">
                  <feGaussianBlur stdDeviation="14"/>
                </filter>
              </defs>
              <rect width="1080" height="1440" fill="url(#lp-paper)"/>
              <rect width="1080" height="1440" filter="url(#lp-grain)" opacity="0.62"/>
              <rect width="1080" height="1440" fill="url(#lp-vign)"/>
              <g fill="#8a7653" filter="url(#lp-blot)">
                <ellipse cx="760" cy="330" rx="170" ry="120" opacity="0.035"/>
                <ellipse cx="300" cy="1180" rx="120" ry="90" opacity="0.03"/>
                <ellipse cx="900" cy="1080" rx="140" ry="100" opacity="0.028"/>
              </g>
              <g stroke="#3c4833" fill="#3c4833">
                <path d="M 236 1452 Q 270 1050 322 700" fill="none" stroke-width="4.4" opacity="0.15"/>
                <line x1="250" y1="1142" x2="262" y2="1140" stroke-width="2.2" opacity="0.15"/>
                <line x1="284" y1="900" x2="296" y2="898" stroke-width="2.2" opacity="0.15"/>
                <path d="M 168 1452 Q 192 1000 240 600" fill="none" stroke-width="3.2" opacity="0.15"/>
                <line x1="188" y1="1128" x2="200" y2="1126" stroke-width="1.8" opacity="0.15"/>
                <line x1="212" y1="846" x2="224" y2="844" stroke-width="1.8" opacity="0.15"/>
                <path d="M 322 1452 Q 336 1210 352 990" fill="none" stroke-width="2.4" opacity="0.13"/>
                <line x1="330" y1="1202" x2="340" y2="1200" stroke-width="1.6" opacity="0.13"/>
                <g opacity="0.20">
                  <path d="M 234 622 C 246 606 264 594 290 584 C 270 606 252 618 234 622"/>
                  <path d="M 228 626 C 218 610 210 590 206 564 C 222 586 228 604 228 626"/>
                  <path d="M 236 628 C 244 642 254 652 268 658 C 254 646 244 636 236 628"/>
                  <path d="M 230 620 C 226 596 234 576 250 556 C 246 580 240 602 230 620"/>
                </g>
                <g opacity="0.20">
                  <path d="M 318 726 C 330 712 348 700 372 690 C 352 712 336 722 318 726"/>
                  <path d="M 312 730 C 302 716 296 700 292 678 C 306 698 312 714 312 730"/>
                  <path d="M 322 732 C 330 744 340 752 352 756 C 340 744 332 736 322 732"/>
                </g>
                <g opacity="0.20">
                  <path d="M 350 992 C 360 978 374 968 392 960 C 376 980 362 990 350 992"/>
                  <path d="M 346 996 C 338 984 334 972 332 954 C 342 972 346 986 346 996"/>
                </g>
              </g>
              <rect x="108" y="108" width="864" height="1224" fill="none" stroke="#a93226" stroke-width="1.6" opacity="0.78"/>
              <rect x="117" y="117" width="846" height="1206" fill="none" stroke="#b03a30" stroke-width="1" opacity="0.45"/>
              <g stroke="#b03a30" stroke-width="1" opacity="0.55">
                <line x1="282.7" y1="117" x2="282.7" y2="1328"/>
                <line x1="411.3" y1="117" x2="411.3" y2="1328"/>
                <line x1="540" y1="117" x2="540" y2="1328"/>
                <line x1="668.7" y1="117" x2="668.7" y2="1328"/>
                <line x1="797.3" y1="117" x2="797.3" y2="1328"/>
              </g>
              <g>
                <rect x="942" y="22" width="58" height="58" rx="5" fill="none" stroke="#a93226" stroke-width="3"/>
                <text x="971" y="63" text-anchor="middle" font-family="'Jiangxi Zhuokai','LXGW WenKai','KaiTi','STKaiti',serif" font-size="23" fill="#a93226">{0}</text>
              </g>
            </svg>
            """;
    }
}
