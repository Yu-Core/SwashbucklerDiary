namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public Task<bool> OpenQQGroup()
        {
            string uri = string.Empty;
#if ANDROID || IOS
            uri = "mqqopensdkapi://bizAgent/qm/qr?url=http%3A%2F%2Fqm.qq.com%2Fcgi-bin%2Fqm%2Fqr%3Ffrom%3Dapp%26p%3Dandroid%26jump_from%3Dwebapi%26k%3DJM3XMjO_Zw-ub2_p1xrk6qVj0_21PeO1";
            return Launcher.Default.TryOpenAsync(uri);
#else
            uri = "http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=gmVF7NgQwU16rrjrTLW37nY9SfDAqKNI&authKey=0tgO1ht368hGRMR0UlEi21vZxKfZdu3h1GmmDyRh4o5qPkDVt0X3RSHoSCiPwkjl&noverify=0&group_code=139864402";
            return Browser.Default.OpenAsync(uri, BrowserLaunchMode.External);
#endif
        }
    }
}
