using RestSharp;
using System;
using System.Linq;
using static CoincheckRequest.EnumExtension;


namespace CoincheckRequest
{
    /// <summary>
    /// coincheckへのリクエスト情報Enum
    /// </summary>
    public enum RequestEnum
    {
        /// <summary>
        /// 板情報
        /// </summary>
        [ActionName("レート"), Path("/api/exchange/orders/rate"), Method(Method.GET)]
        Rate,
        /// <summary>
        /// 板情報
        /// </summary>
        [ActionName("ティッカー"), Path("/api/ticker"), Method(Method.GET)]
        Ticker,
        /// <summary>
        /// 残高照会
        /// </summary>
        [ActionName("残高照会"), Path("/api/accounts/balance"), Method(Method.GET)]
        Balance,
        /// <summary>
        /// 取引履歴
        /// </summary>
        [ActionName("取引履歴"), Path("/api/exchange/orders/transactions"), Method(Method.GET)]
        Transactions,
        /// <summary>
        /// 板情報
        /// </summary>
        [ActionName("板情報"), Path("/api/order_books"), Method(Method.GET)]
        Board,
        /// <summary>
        /// 注文一覧（未決済）
        /// </summary>
        [ActionName("発注一覧"), Path("/api/exchange/orders/opens"), Method(Method.GET)]
        ExchangeList,
        /// <summary>
        /// 発注売り注文
        /// </summary>
        [ActionName("売り注文"), Path("/api/exchange/orders"), Method(Method.POST)]
        ExchangeSaleOrder,
        /// <summary>
        /// 発注買い注文
        /// </summary>
        [ActionName("買い注文"), Path("/api/exchange/orders"), Method(Method.POST)]
        ExchangeBuyOrder,
        /// <summary>
        /// キャンセル
        /// </summary>
        [ActionName("注文キャンセル"), Path("/api/exchange/orders/"), Method(Method.DELETE)]
        ExchangeCanxel,
    }

    static class EnumExtension
    {
        /// <summary>
        /// Path属性
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        public sealed class PathAttribute : Attribute
        {
            public string Path { get; private set; }
            public PathAttribute(string color)
            {
                this.Path = color;
            }
        }

        public static string GetPath(this Enum value) => value.GetAttribute<PathAttribute>()?.Path ?? string.Empty;

        /// <summary>
        /// Method属性
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        public sealed class MethodAttribute : Attribute
        {
            public Method Method { get; private set; }
            public MethodAttribute(Method color)
            {
                this.Method = color;
            }
        }

        public static Method GetMethod(this Enum value) => value.GetAttribute<MethodAttribute>()?.Method ?? Method.GET;

        /// <summary>
        /// 処理名称
        /// </summary>
        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        public sealed class ActionNameAttribute : Attribute
        {
            public string ActionName { get; private set; }
            public ActionNameAttribute(string color)
            {
                this.ActionName = color;
            }
        }

        public static string GetActionName(this Enum value) => value.GetAttribute<ActionNameAttribute>()?.ActionName ?? string.Empty;

        /// <summary>
        /// 特定の属性を取得する
        /// </summary>
        /// <typeparam name="TAttribute">属性型</typeparam>
        private static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            //リフレクションを用いて列挙体の型から情報を取得
            var fieldInfo = value.GetType().GetField(value.ToString());
            //指定した属性のリスト
            var attributes
                = fieldInfo.GetCustomAttributes(typeof(TAttribute), false)
                .Cast<TAttribute>();
            //属性がなかった場合、空を返す
            if ((attributes?.Count() ?? 0) <= 0)
                return null;
            //同じ属性が複数含まれていても、最初のみ返す
            return attributes.First();
        }
    }
}
