using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Widget;
using static Android.Resource;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

namespace SwashbucklerDiary.Platforms.Android
{
    #nullable disable
    public class GlobalLayoutUtil
    {
        private bool isImmersed = false;
        private View mChildOfContent;
        private FrameLayout.LayoutParams frameLayoutParams;
        private int usableHeightPrevious = 0;

        public static void AssistActivity(Activity activity)
        {
            _ = new GlobalLayoutUtil(activity);
        }

        public GlobalLayoutUtil(Activity activity)
        {
            FrameLayout content = (FrameLayout)activity.FindViewById(Id.Content);
            mChildOfContent = content.GetChildAt(0);
            mChildOfContent.ViewTreeObserver.GlobalLayout += (s, o) => PossiblyResizeChildOfContent(activity);
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
        }

        private void PossiblyResizeChildOfContent(Activity activity)
        {
            //当前可视区域的高度
            int usableHeightNow = ComputeUsableHeight();
            //当前高度值和之前的进行对比,变化将进行重新绘制
            if (usableHeightNow != usableHeightPrevious)
            {
                //获取当前屏幕高度
                //Ps: 并不是真正的屏幕高度，是当前app的窗口高度，分屏时的高度为分屏窗口高度
                int usableHeightSansKeyboard = mChildOfContent.RootView.Height;
                // 高度差值：屏幕高度 - 可视内容高度
                int heightDifference = usableHeightSansKeyboard - usableHeightNow;
                // 差值为负，说明获取屏幕高度时出错，宽高状态值反了，重新计算
                if (heightDifference < 0)
                {
                    usableHeightSansKeyboard = mChildOfContent.RootView.Width;
                    heightDifference = usableHeightSansKeyboard - usableHeightNow;
                }
                // keyboard probably just became visible
                // 如果差值大于屏幕高度的 1/4，则认为输入软键盘为弹出状态
                if (heightDifference > usableHeightSansKeyboard / 4)
                {
                    // 设置布局高度为：屏幕高度 - 高度差
                    frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference;
                }
                //else if (heightDifference >= GetNavigationBarHeight(activity))
                //{
                //    // keyboard probably just became hidden
                //    // 虚拟导航栏显示
                //    frameLayoutParams.Height = usableHeightNow - GetNavigationBarHeight(activity);
                //}
                else
                {// 其他情况直接设置为可视高度即可
                    frameLayoutParams.Height = usableHeightNow;
                }
            }
            // 刷新布局，会重新测量、绘制
            mChildOfContent.RequestLayout();
            // 保存高度信息
            usableHeightPrevious = usableHeightNow;
        }


        /**
         * 获取可视内容区域的高度
         */
        private int ComputeUsableHeight()
        {
            Rect rect = new Rect();
            mChildOfContent.GetWindowVisibleDisplayFrame(rect);
            if (isImmersed)
                return (int)rect.Bottom;
            else
                return (int)(rect.Bottom - rect.Top);
        }

        /**
         * 获取导航栏的真实高度
         *
         * @param context:
         * @return: 导航栏高度
         */
        private static int GetNavigationBarHeight(Context context)
        {
            int result = 0;
            Resources resources = context.Resources;
            int resourceId =
                    resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            if (resourceId > 0)
                result = resources.GetDimensionPixelSize(resourceId);
            return result;
        }

    }
}