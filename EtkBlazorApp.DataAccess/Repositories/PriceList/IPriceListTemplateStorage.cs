﻿using EtkBlazorApp.DataAccess.Entity;
using EtkBlazorApp.DataAccess.Entity.PriceList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtkBlazorApp.DataAccess.Repositories.PriceList
{
    //TODO: разбить на мелкие интерфейсы
    public interface IPriceListTemplateStorage
    {
        Task CreatePriceList(PriceListTemplateEntity data);
        Task UpdatePriceList(PriceListTemplateEntity data);
        Task DeletePriceList(string guid);
        Task ChangePriceListTemplateDiscount(string guid, decimal discount);
        Task<List<PriceListTemplateEntity>> GetPriceListTemplates();
        Task<PriceListTemplateEntity> GetPriceListTemplateById(string guid);
        Task<List<PriceListTemplateRemoteUriMethodEntity>> GetPricelistTemplateRemoteLoadMethods();
        Task<List<string>> GetPriceListTemplatGroupNames();
    }


    public class PriceListTemplateStorage : IPriceListTemplateStorage
    {
        private readonly IDatabaseAccess database;

        public PriceListTemplateStorage(IDatabaseAccess database)
        {
            this.database = database;
        }

        public async Task<PriceListTemplateEntity> GetPriceListTemplateById(string guid)
        {
            string sql = @"SELECT t.*, 
                                    lm.name as remote_uri_method_name, 
                                    cred.login as credentials_login, cred.password as credentials_password,
                                    esc.subject as email_criteria_subject, 
                                    esc.sender as email_criteria_sender, 
                                    esc.file_name_pattern as email_criteria_file_name_pattern,
                                    esc.max_age_in_days as email_criteria_max_age_in_days

                          FROM etk_app_price_list_template t
                              LEFT JOIN etk_app_price_list_template_load_method lm ON t.remote_uri_method_id = lm.id 
                              LEFT JOIN etk_app_price_list_template_email_search_criteria esc ON t.id = esc.template_guid 
                              LEFT JOIN etk_app_price_list_template_credentials cred ON t.id = cred.template_guid 
                          WHERE t.id = @guid 
                          LIMIT 1";

            var templateInfo = await database.GetFirstOrDefault<PriceListTemplateEntity, dynamic>(sql, new { guid });

            //Дополнительные данные шаблона
            templateInfo.quantity_map = await GetQuantityMapRecordsForTemplate(guid);
            templateInfo.model_map = await GetModelMapRecordsForTemplate(guid);
            templateInfo.manufacturer_name_map = await GetManufacturerNameMapRecordsForTemplate(guid);
            templateInfo.manufacturer_skip_list = await GetManufacturerSkipRecordsForTemplate(guid);
            templateInfo.manufacturer_discount_map = await GetDiscountMapRecordsForTemplate(guid);
            templateInfo.manufacturer_purchase_map = await GetPurchaseMapRecordsForTemplate(guid);

            return templateInfo;
        }

        private async Task<List<ModelMapRecordEntity>> GetModelMapRecordsForTemplate(string guid)
        {
            string sql = "SELECT * FROM etk_app_price_list_template_model_map WHERE price_list_guid = @guid";
            var data = await database.GetList<ModelMapRecordEntity, dynamic>(sql, new { guid });
            return data;
        }

        private async Task<List<QuantityMapRecordEntity>> GetQuantityMapRecordsForTemplate(string guid)
        {
            string sql = "SELECT * FROM etk_app_price_list_template_quantity_map WHERE price_list_guid = @guid";
            var data = await database.GetList<QuantityMapRecordEntity, dynamic>(sql, new { guid });
            return data;
        }

        private async Task<List<ManufacturerSkipRecordEntity>> GetManufacturerSkipRecordsForTemplate(string guid)
        {
            string sql = @"SELECT t.*, m.name
                           FROM etk_app_price_list_template_manufacturer_list t
                           LEFT JOIN oc_manufacturer m ON (t.manufacturer_id = m.manufacturer_id)
                           WHERE price_list_guid = @guid";
            var data = await database.GetList<ManufacturerSkipRecordEntity, dynamic>(sql, new { guid });
            return data;
        }

        private async Task<List<ManufacturerDiscountMapEntity>> GetDiscountMapRecordsForTemplate(string guid)
        {
            string sql = @"SELECT t.*, m.name
                           FROM etk_app_price_list_template_discount_map t
                           LEFT JOIN oc_manufacturer m ON (t.manufacturer_id = m.manufacturer_id)
                           WHERE price_list_guid = @guid";
            var data = await database.GetList<ManufacturerDiscountMapEntity, dynamic>(sql, new { guid });
            return data;
        }

        private async Task<List<ManufacturerDiscountMapEntity>> GetPurchaseMapRecordsForTemplate(string guid)
        {
            string sql = @"SELECT t.*, m.name
                           FROM etk_app_price_list_template_purchase_discount t
                           LEFT JOIN oc_manufacturer m ON (t.manufacturer_id = m.manufacturer_id)
                           WHERE price_list_guid = @guid";
            var data = await database.GetList<ManufacturerDiscountMapEntity, dynamic>(sql, new { guid });
            return data;
        }

        private async Task<List<ManufacturerMapRecordEntity>> GetManufacturerNameMapRecordsForTemplate(string guid)
        {
            string sql = @"SELECT t.*, m.name
                          FROM etk_app_price_list_template_manufacturer_map t
                          LEFT JOIN oc_manufacturer m ON (t.manufacturer_id = m.manufacturer_id)
                          WHERE price_list_guid = @guid";
            var data = await database.GetList<ManufacturerMapRecordEntity, dynamic>(sql, new { guid });
            return data;
        }

        public async Task<List<PriceListTemplateEntity>> GetPriceListTemplates()
        {
            string sql = @"SELECT t.*, lm.name as remote_uri_method_name, esc.sender as email_criteria_sender
                          FROM etk_app_price_list_template t
                          LEFT JOIN etk_app_price_list_template_load_method lm ON t.remote_uri_method_id = lm.id
                          LEFT JOIN etk_app_price_list_template_email_search_criteria esc ON t.id = esc.template_guid";

            var templatesInfo = await database.GetList<PriceListTemplateEntity, dynamic>(sql, new { });
            return templatesInfo;
        }

        public async Task ChangePriceListTemplateDiscount(string id, decimal discount)
        {
            string sql = "UPDATE etk_app_price_list_template SET discount = @discount WHERE id = @id";
            await database.ExecuteQuery(sql, new { id, discount });
        }

        public async Task<List<PriceListTemplateRemoteUriMethodEntity>> GetPricelistTemplateRemoteLoadMethods()
        {
            string sql = "SELECT * FROM etk_app_price_list_template_load_method";
            var data = await database.GetList<PriceListTemplateRemoteUriMethodEntity>(sql);
            return data;
        }

        public async Task<List<string>> GetPriceListTemplatGroupNames()
        {
            string sql = "SELECT DISTINCT group_name FROM etk_app_price_list_template ORDER BY group_name";
            var data = await database.GetList<string>(sql);
            return data;
        }

        public async Task UpdatePriceList(PriceListTemplateEntity data)
        {
            string sql = @"UPDATE etk_app_price_list_template
                           SET id = @id,
                                stock_partner_id = @stock_partner_id,
                                title = @title,
                                description = @description,
                                group_name = @group_name,
                                remote_uri = @remote_uri,
                                remote_uri_method_id = @remote_uri_method_id,
                                nds = @nds,
                                discount = @discount,
                                image = @image
                           WHERE id = @id";

            await database.ExecuteQuery(sql, data);

            if (data.credentials_login != null && data.credentials_password != null)
            {
                await InsertOrUpdatePriceListCredentials(data);
            }
            if (data.email_criteria_sender != null)
            {
                await InsertOrUpdatePriceListEmailSearchCriteria(data);
            }
        }

        private async Task InsertOrUpdatePriceListEmailSearchCriteria(PriceListTemplateEntity data)
        {
            string sql = @"INSERT INTO etk_app_price_list_template_email_search_criteria 
                            (template_guid, subject, sender, file_name_pattern, max_age_in_days) VALUES
                            (@id, @email_criteria_subject, @email_criteria_sender, @email_criteria_file_name_pattern, @email_criteria_max_age_in_days)
                          ON DUPLICATE KEY UPDATE
                          subject = @email_criteria_subject, 
                              sender = @email_criteria_sender, 
                              file_name_pattern = @email_criteria_file_name_pattern,
                              max_age_in_days = @email_criteria_max_age_in_days";

            await database.ExecuteQuery(sql, data);
        }

        private async Task InsertOrUpdatePriceListCredentials(PriceListTemplateEntity data)
        {
            string sql = @"INSERT INTO etk_app_price_list_template_credentials (template_guid, login, password) VALUES
                            (@id, @credentials_login, @credentials_password)
                          ON DUPLICATE KEY UPDATE
                          login = @credentials_login, password = @credentials_password";

            await database.ExecuteQuery(sql, data);
        }

        public async Task CreatePriceList(PriceListTemplateEntity data)
        {
            string sql = @"INSERT INTO etk_app_price_list_template
                        (id, stock_partner_id, title, description, group_name, remote_uri, remote_uri_method_id, nds, discount, image) VALUES
                        (@id, @stock_partner_id, @title, @description, @group_name, @remote_uri, @remote_uri_method_id, @nds, @discount, @image)";

            await database.ExecuteQuery(sql, data);

            if (data.credentials_login != null && data.credentials_password != null)
            {
                await InsertOrUpdatePriceListCredentials(data);
            }
            if (data.email_criteria_sender != null)
            {
                await InsertOrUpdatePriceListEmailSearchCriteria(data);
            }

        }

        public async Task DeletePriceList(string guid)
        {
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_template WHERE id = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_email_search_criteria WHERE template_id = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_credentials WHERE template_id = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_manufacturer_list WHERE price_list_guid = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_manufacturer_map WHERE price_list_guid = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_quantity_map WHERE price_list_guid = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_model_map WHERE price_list_guid = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_discount_map WHERE price_list_guid = @guid", new { guid });
            await database.ExecuteQuery("DELETE FROM etk_app_price_list_template_purchase_discount WHERE price_list_guid = @guid", new { guid });
        }

    }
}
