﻿using EtkBlazorApp.DataAccess.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtkBlazorApp.DataAccess
{
    public interface IPrikatTemplateStorage
    {
        Task<List<PrikatReportTemplateEntity>> GetPrikatTemplates();
        Task SavePrikatTemplate(PrikatReportTemplateEntity template);
    }

    public class PrikatTemplateStorage : IPrikatTemplateStorage
    {
        private readonly IDatabaseAccess database;

        public PrikatTemplateStorage(IDatabaseAccess database)
        {
            this.database = database;
        }

        public async Task<List<PrikatReportTemplateEntity>> GetPrikatTemplates()
        {
            string sql = @"SELECT t.*, m.manufacturer_id as manufacturer_id,  m.name as manufacturer_name
                           FROM etk_app_prikat_template t
                           RIGHT JOIN oc_manufacturer m ON t.manufacturer_id = m.manufacturer_id";

            var templatesInfo = await database.GetList<PrikatReportTemplateEntity, dynamic>(sql, new { });
            return templatesInfo;
        }

        public async Task SavePrikatTemplate(PrikatReportTemplateEntity template)
        {
            if(template.template_id == 0)
            {
                string insertSql = @"INSERT INTO etk_app_prikat_template
                       (manufacturer_id, enabled, discount1, discount2, currency_code)
                       VALUES
                       (@manufacturer_id, @enabled, @discount1, @discount2, @currency_code)";
                await database.ExecuteQuery(insertSql, template);
                template.template_id = await database.GetScalar<int>("SELECT max(template_id) FROM etk_app_prikat_template");
            }
            else
            {
                string updateSql = @"UPDATE etk_app_prikat_template
                                    SET 
                                        manufacturer_id = @manufacturer_id,
                                        enabled = @enabled,
                                        discount1 = @discount1,
                                        discount2 = @discount2,
                                        currency_code = @currency_code
                                    WHERE template_id = @template_id";

                await database.ExecuteQuery(updateSql, template);
            }        
        }

    }
}
