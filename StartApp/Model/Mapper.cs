using AutoMapper;
using CommonUITools.Converter;
using CommonUITools.Utils;

namespace StartApp.Model;

public class Mapper {
    public static IMapper Instance => CommonUtils.GetSingletonInstance<IMapper>(typeof(Mapper), () => {
        return new MapperConfiguration(cfg => {
            cfg.ClearPrefixes();
            cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
            MapperUtils.AddCommonConverters(cfg);
            cfg.AddMaps(typeof(AppTaskProfile));
        }).CreateMapper();
    });

    /// <summary>
    /// 包装 Dispatcher.Invoke Map 方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T Map<T>(object obj) {
        return App.Current.Dispatcher.Invoke(() => Instance.Map<T>(obj));
    }
}


