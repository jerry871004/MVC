using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Models
{
    public class BookService
    {
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }

        /// <summary>
        /// Dropdownlist 圖書類別
        /// </summary>
        public List<SelectListItem> GetClassTable(string arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT BOOK_CLASS_ID AS BookClassId,
		                          BOOK_CLASS_NAME AS BookClassName
                           FROM BOOK_CLASS
                           ORDER BY BookClassName";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapClassData(dt, arg);
        }

        private List<SelectListItem> MapClassData(DataTable dt, string arg)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Value = row["BookClassId"].ToString(),
                    Text = row["BookClassName"].ToString()                
                });
            }
            return result;
        }
        /// <summary>
        /// Dropdownlist 借閱人
        /// </summary>
        public List<SelectListItem> GetUserTable(string arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT [USER_ID] AS KeeperId,
		                          [USER_ENAME] AS Keeper
		                   FROM MEMBER_M";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapUserData(dt, arg);
        }

        private List<SelectListItem> MapUserData(DataTable dt, string arg)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Value = row["KeeperId"].ToString(),
                    Text = row["Keeper"].ToString()
                });
            }
            return result;
        }
        /// <summary>
        /// Dropdownlist 借閱狀態
        /// </summary>
        public List<SelectListItem> GetStatusTable(string arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT   CODE_ID AS StatusId,
		                            CODE_NAME AS StatusName
                             FROM BOOK_CODE
                             WHERE CODE_TYPE = 'BOOK_STATUS'";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapStatusData(dt, arg);
        }

        private List<SelectListItem> MapStatusData(DataTable dt, string arg)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Value = row["StatusId"].ToString(),
                    Text = row["StatusName"].ToString()              
                });
            }
            return result;
        }

        public List<Models.BOOK> GetBookData(Models.BookSearch arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT   BD.BOOK_CLASS_ID AS BookClassId,
		                            BCL.BOOK_CLASS_NAME AS BookClassName,
		                            BD.BOOK_ID AS BookId,
		                            BD.BOOK_NAME AS BookName,
		                            ISNULL(BD.BOOK_KEEPER, '') AS KeeperId,
		                            ISNULL(MM.USER_ENAME, '') AS Keeper,
		                            BD.BOOK_STATUS AS BookStatusId,
		                            BCO.CODE_NAME AS BookStatus,
		                            FORMAT(BD.BOOK_BOUGHT_DATE, 'yyyy/MM/dd') AS BuyDate
                            FROM BOOK_DATA AS BD
                            INNER JOIN BOOK_CLASS BCL ON BD.BOOK_CLASS_ID = BCL.BOOK_CLASS_ID
                            INNER JOIN BOOK_CODE AS BCO ON BD.BOOK_STATUS = BCO.CODE_ID AND BCO.CODE_TYPE = 'BOOK_STATUS'
                            LEFT JOIN MEMBER_M AS MM ON BD.BOOK_KEEPER = MM.[USER_ID]
                            WHERE BD.BOOK_NAME LIKE '%'+ @BookName + '%' AND
		                            BD.BOOK_CLASS_ID LIKE @BookClassId+'%' AND
		                            ISNULL(BD.BOOK_KEEPER, '') LIKE @KeeperId+'%' AND
		                            BD.BOOK_STATUS LIKE '%'+@BookStatusId+'%'
                            ORDER BY BookId";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClassId == null ? string.Empty : arg.BookClassId));
                cmd.Parameters.Add(new SqlParameter("@KeeperId", arg.KeeperId == null ? string.Empty : arg.KeeperId));
                cmd.Parameters.Add(new SqlParameter("@BookStatusId", arg.BookStatusId == null ? string.Empty : arg.BookStatusId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookData(dt);
        }
        private List<Models.BOOK> MapBookData(DataTable table)
        {
            List<Models.BOOK> result = new List<BOOK>();
            foreach(DataRow row in table.Rows)
            {
                result.Add(new BOOK()
                {
                    BookName = row["BookName"].ToString(),
                    BookId = (int)row["BookId"],
                    BookClassName = row["BookClassName"].ToString(),
                    BookClassId = row["BookClassId"].ToString(),
                    BookStatusId = row["BookStatusId"].ToString(),
                    BookStatus = row["BookStatus"].ToString(),
                    KeeperId = row["KeeperId"].ToString(),
                    Keeper = row["Keeper"].ToString(),
                    BuyDate = row["BuyDate"].ToString()
                });
            }
            return result;
        }

        public void DeleteBook(int arg)
        {
            DataTable dt = new DataTable();
            string sql = @"DELETE BOOK_DATA WHERE BOOK_ID = @BookId";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId",(int)arg));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
        }

        public bool InsertBookInfo(Models.BOOK arg)
        {
            if (arg.BookName == null || arg.Author == null || arg.Publisher == null || arg.Introduction == null || arg.BuyDate == null || arg.BookClassId == null)
            {
                return false;
            }

            DataTable dt = new DataTable();
            string sql = @"INSERT INTO BOOK_DATA (BOOK_NAME, BOOK_AUTHOR, BOOK_PUBLISHER, BOOK_NOTE, BOOK_BOUGHT_DATE, BOOK_CLASS_ID, BOOK_STATUS)
		                         VALUES (@BookName, @Author, @Publisher, @Introduction, CONVERT(DATETIME, @BuyDate), @BookClassId, 'A')";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@Author", arg.Author == null ? string.Empty : arg.Author));
                cmd.Parameters.Add(new SqlParameter("@Publisher", arg.Publisher == null ? string.Empty : arg.Publisher));
                cmd.Parameters.Add(new SqlParameter("@Introduction", arg.Introduction == null ? string.Empty : arg.Introduction));
                cmd.Parameters.Add(new SqlParameter("@BuyDate", arg.BuyDate == null ? string.Empty : arg.BuyDate));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClassId == null ? string.Empty : arg.BookClassId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return true;
        }
    }
}