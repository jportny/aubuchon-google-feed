using Mozu.Api.ToolKit.Config;

namespace Mozu.AubuchonDataAdapter.Domain
{
    public static class MzdbHelper
    {
        public static string GetListNameSpace(IAppSetting appSetting)
        {
            return appSetting.Namespace;
        }

        public static string GetListFullName(IAppSetting appSetting, string listName)
        {
            var ns = GetListNameSpace(appSetting);
            return string.Format("{0}@{1}", listName, ns);
        }

    }
}
