using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoredProcedureLanguage.Data;
using StoredProcedureLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoredProcedureLanguage.Controllers
{
    public class LanguageController : Controller
    {
        public StoredProcedureDBContext _context;
        public IConfiguration _config { get; }
        public LanguageController
            (
            StoredProcedureDBContext context, IConfiguration config
            )
        {
            _context = context;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IEnumerable<Language> SearchResult()
        {
            var result = _context.Language
                .FromSqlRaw<Language>("dbo.SearchLanguages")
                .ToList();

            return result;
        }
        [HttpGet]
        public IActionResult DynamicSQL()
        {
            string connectionStr = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "dbo.SearchLanguages";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<Language> model = new List<Language>();
                while (sdr.Read())
                {
                    var details = new Language();
                    details.Name = sdr["Name"].ToString();
                    details.Family = sdr["Family"].ToString();
                    details.MainAreal = sdr["MainAreal"].ToString();
                    details.Speakers = Convert.ToInt32(sdr["Speakers"]);
                    model.Add(details);
                }
                return View(model);
            }


        }
        /// <summary>
        /// SearchPageWithoutDynamicSQL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DynamicSQL(string name, string family, string mainAreal, int speakers)
        {
            string connectionStr = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "dbo.SearchLanguages";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (name != null)
                {
                    SqlParameter param_fn = new SqlParameter("@Name", name);
                    cmd.Parameters.Add(param_fn);
                }
                if (family != null)
                {
                    SqlParameter param_ln = new SqlParameter("@Family", family);
                    cmd.Parameters.Add(param_ln);
                }
                if (mainAreal != null)
                {
                    SqlParameter param_g = new SqlParameter("@MainAreal", mainAreal);
                    cmd.Parameters.Add(param_g);
                }
                if (speakers != 0)
                {
                    SqlParameter param_s = new SqlParameter("@Speakers", speakers);
                    cmd.Parameters.Add(param_s);
                }
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<Language> model = new List<Language>();
                while (sdr.Read())
                {
                    var details = new Language();
                    details.Name = sdr["Name"].ToString();
                    details.Family = sdr["Family"].ToString();
                    details.MainAreal = sdr["MainAreal"].ToString();
                    details.Speakers = Convert.ToInt32(sdr["Speakers"]);
                    model.Add(details);
                }
                return View(model);
            }
        }
    }
}
