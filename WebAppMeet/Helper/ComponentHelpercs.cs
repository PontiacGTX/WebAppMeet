using Microsoft.JSInterop;

namespace WebAppMeet.Helper
{
    public static  class ComponentHelpercs
    {
        [JSInvokable]
        public static void ReturnDataAsync()
        {
            Console.WriteLine("Page will be refreshed");
        }
    }
}
