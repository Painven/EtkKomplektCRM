﻿using EtkBlazorApp.BL;
using EtkBlazorApp.Core.Data;
using EtkBlazorApp.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace EtkBlazorApp.Services;

public class CurrencyCheckerCbRf : ICurrencyChecker
{
    Dictionary<CurrencyType, decimal> rates;
    public DateTime LastUpdate { get; private set; }
    private readonly IMemoryCache cache;

    public CurrencyCheckerCbRf(IMemoryCache cache)
    {
        this.cache = cache;
    }

    public async ValueTask<decimal> GetCurrencyRate(CurrencyType type)
    {
        if (!cache.TryGetValue(nameof(rates), out rates))
        {
            rates = await ReadCurrenciesFromCbRf();
            LastUpdate = DateTime.Now;

            cache.Set(nameof(rates), rates, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
        }

        return (rates != null && rates.ContainsKey(type)) ? rates[type] : 0;
    }

    private async Task<Dictionary<CurrencyType, decimal>> ReadCurrenciesFromCbRf()
    {
        var dic = new Dictionary<CurrencyType, decimal>();
        //Инициализируем объекта типа XmlTextReader и
        //загружаем XML документ с сайта центрального банка
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

        var xmlStreamTask = httpClient.GetStreamAsync("http://www.cbr.ru/scripts/XML_daily.asp");
        var delayTask = Task.Delay(TimeSpan.FromSeconds(10));

        var winner = await Task.WhenAny(xmlStreamTask, delayTask);
        if (winner == delayTask)
        {
            throw new TimeoutException("Не удалось получить цены с сайта ЦБ РФ");
        }

        string USDXml, EuroXML, YanXML;
        ReadXmlData(xmlStreamTask.Result, out USDXml, out EuroXML, out YanXML);

        //Из выдернутых кусков XML кода создаем новые XML документы
        XmlDocument usdXmlDocument = new XmlDocument();
        usdXmlDocument.LoadXml(USDXml);
        XmlDocument euroXmlDocument = new XmlDocument();
        euroXmlDocument.LoadXml(EuroXML);
        XmlDocument yanXmlDocument = new XmlDocument();
        yanXmlDocument.LoadXml(YanXML);

        //Метод возвращает узел, соответствующий выражению XPath
        XmlNode xmlNode = usdXmlDocument.SelectSingleNode("Valute/Value");
        decimal usdValue = Convert.ToDecimal(xmlNode.InnerText);

        xmlNode = euroXmlDocument.SelectSingleNode("Valute/Value");
        decimal euroValue = Convert.ToDecimal(xmlNode.InnerText);

        xmlNode = yanXmlDocument.SelectSingleNode("Valute/Value");
        decimal yanValue = Convert.ToDecimal(xmlNode.InnerText);

        dic[CurrencyType.USD] = usdValue;
        dic[CurrencyType.EUR] = euroValue;
        dic[CurrencyType.CNY] = yanValue;
        dic[CurrencyType.RUB] = 1;


        return dic;
    }

    private static void ReadXmlData(Stream xmlStreamTask, out string USDXml, out string EuroXML, out string UanXML)
    {
        XmlTextReader reader = new XmlTextReader(xmlStreamTask);
        //В эти переменные будем сохранять куски XML
        //с определенными валютами (Euro, USD)
        USDXml = "";
        EuroXML = "";
        UanXML = "";

        //Перебираем все узлы в загруженном документе
        while (reader.Read())
        {
            //Проверяем тип текущего узла
            switch (reader.NodeType)
            {
                //Если этого элемент Valute, то начинаем анализировать атрибуты
                case XmlNodeType.Element:

                    if (reader.Name == "Valute")
                    {
                        if (reader.HasAttributes)
                        {
                            //Метод передвигает указатель к следующему атрибуту
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == "ID")
                                {
                                    if (reader.Value == "R01235")
                                    {
                                        reader.MoveToElement();
                                        USDXml = reader.ReadOuterXml();
                                    }
                                }

                                if (reader.Name == "ID")
                                {
                                    if (reader.Value == "R01239")
                                    {
                                        reader.MoveToElement();
                                        EuroXML = reader.ReadOuterXml();
                                    }
                                }

                                if (reader.Name == "ID")
                                {
                                    if (reader.Value == "R01375")
                                    {
                                        reader.MoveToElement();
                                        UanXML = reader.ReadOuterXml();
                                    }
                                }
                            }
                        }
                    }

                    break;
            }
        }
    }
}

