using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcCreate.Jklss.Model.Market
{
    public class MarketModel
    {
    }

    public class UserInfo
    {
        public string UserImage { get; set; }

        public string UserName { get; set; }

        public int UserPoints { get; set; }

        public int UserGroup { get; set; }

        public int UserVip { get; set; }

        public string UserAccets { get; set; }
    }

    public class PageList
    {
        public int page { get; set; }

        public int size { get; set; }
    }

    public class CommodityModel
    {
        public string Name { get; set; }

        public int NeedPoints { get; set; }

        public string Introduction { get; set; }
    }

    public class arccreate_market_commodity
    {
        /// <summary>
        /// 市场内容ID
        /// </summary>
        public int mid { get; set; }

        /// <summary>
        /// 0为数据、1为语法模型、2为定制
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 是否为精华（表置顶的意思）
        /// </summary>
        public bool iscream { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 作者uid
        /// </summary>
        public int userid { get; set; }

        /// <summary>
        /// 购买需要的点数
        /// </summary>
        public int need_points { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string introduction { get; set; }

        /// <summary>
        /// 数据存储ID
        /// </summary>
        public int save_data_id { get; set; }

        /// <summary>
        /// 平均点赞
        /// </summary>
        public int star { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_time { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }
    }
}
