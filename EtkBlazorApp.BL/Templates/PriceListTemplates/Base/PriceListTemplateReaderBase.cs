﻿using EtkBlazorApp.DataAccess.Entity.PriceList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EtkBlazorApp.BL.Templates.PriceListTemplates.Base
{
    public abstract class PriceListTemplateReaderBase
    {
        //TODO: Тут стоит поменят словари и списки на ReadOnly и/или вынести менеджер (что бы обрабатывало за пределами шаблона, там же где ModelMap)
        protected Dictionary<string, string> ManufacturerNameMap { get; private set; }
        protected Dictionary<string, int> QuantityMap { get; private set; }
        protected Dictionary<string, decimal> ManufacturerDiscountMap { get; private set; }
        protected List<string> BrandsWhiteList { get; private set; }
        protected List<string> BrandsBlackList { get; private set; }

        public PriceListTemplateReaderBase()
        {
            ManufacturerNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            QuantityMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            ManufacturerDiscountMap = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            BrandsWhiteList = new List<string>();
            BrandsBlackList = new List<string>();
        }

        protected string MapManufacturerName(string manufacturerName)
        {
            if (!string.IsNullOrWhiteSpace(manufacturerName) && ManufacturerNameMap.Any() && ManufacturerNameMap.ContainsKey(manufacturerName))
            {
                return ManufacturerNameMap[manufacturerName];
            }
            return manufacturerName;
        }

        protected virtual decimal? ParsePrice(string str, bool canBeNull = false, int? roundDigits = null)
        {
            decimal? price = null;
            if (!string.IsNullOrWhiteSpace(str))
            {
                string strValue = str.Replace(",", ".").Replace(" ", string.Empty);
                if (decimal.TryParse(strValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsedPrice))
                {
                    price = Math.Max(parsedPrice, 0);
                    if (roundDigits.HasValue)
                    {
                        price = Math.Round(price.Value, roundDigits.Value);
                    }
                }
            }

            return canBeNull ? price : price ?? 0;
        }

        protected virtual int? ParseQuantity(string str, bool canBeNull = false)
        {
            int? quantity = null;
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (decimal.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsedQuantity))
                {
                    quantity = Math.Max((int)parsedQuantity, 0);
                }
                else if (QuantityMap.ContainsKey(str))
                {
                    quantity = QuantityMap[str];
                }
            }

            return canBeNull ? quantity : quantity ?? 0;
        }

        //TODO: убрать вызов метода из шаблонов и оставить только в окончательной выгрузке
        //но тогда получается что будет тратиться лишняя память для огромных прайс-листов, в которых можно пропустить сразу
        /// <summary>
        /// Проверка на пропуск из загрузки прайс-листа 
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        protected bool SkipThisBrand(string manufacturer)
        {
            bool blackListCondition = BrandsBlackList.Any() && BrandsBlackList.Contains(manufacturer, StringComparer.OrdinalIgnoreCase);
            bool whiteListCondition = BrandsWhiteList.Any() && BrandsWhiteList.Contains(manufacturer, StringComparer.OrdinalIgnoreCase) == false;

            return blackListCondition || whiteListCondition;
        }

        public void FillTemplateInfo(PriceListTemplateEntity templateInfo)
        {
            if (templateInfo.quantity_map != null)
            {
                foreach (var kvp in templateInfo.quantity_map)
                {
                    QuantityMap[kvp.text] = kvp.quantity;
                }
            }
            if (templateInfo.manufacturer_name_map != null)
            {
                foreach (var kvp in templateInfo.manufacturer_name_map)
                {
                    ManufacturerNameMap[kvp.text] = kvp.name;
                }
            }
            if (templateInfo.manufacturer_discount_map != null)
            {
                foreach (var kvp in templateInfo.manufacturer_discount_map)
                {
                    ManufacturerDiscountMap[kvp.name] = kvp.discount;
                }
            }
            if (templateInfo.manufacturer_skip_list != null)
            {
                var listsSource = templateInfo.manufacturer_skip_list
                    .Where(i => !string.IsNullOrWhiteSpace(i.name) && !string.IsNullOrWhiteSpace(i.list_type));

                foreach (var item in listsSource)
                {
                    if (item.list_type == "black_list" && !BrandsBlackList.Contains(item.name))
                    {
                        BrandsBlackList.Add(item.name);
                    }
                    if (item.list_type == "white_list" && !BrandsWhiteList.Contains(item.name))
                    {
                        BrandsWhiteList.Add(item.name);
                    }
                }
            }
        }
    }
}
