using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EBill.Models
{
    public class BillDAL
    {
        SqlCommand cmd;
        SqlConnection con;
        public BillDAL()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString;
            con = new SqlConnection(ConStr);
        }
        public void InsertBillDetails(BillDetail details)
        {
            try
            {
                details.TotalAmount = details.Items.Sum(i => i.Price * i.Quantity);
                con.Open();
                cmd = new SqlCommand("spt_InsertEBillDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerName", details.CustomerName);
                cmd.Parameters.AddWithValue("@MobileNumber", details.MobileNumber);
                cmd.Parameters.AddWithValue("@Address", details.Address);
                cmd.Parameters.AddWithValue("TotalAmount", details.TotalAmount);

                SqlParameter sp = new SqlParameter();
                sp.DbType = DbType.Int32;
                sp.Direction = ParameterDirection.Output;
                sp.ParameterName = "@Id";
                cmd.Parameters.Add(sp);
                cmd.ExecuteNonQuery();
                int id = int.Parse(sp.Value.ToString());
                if (details.Items.Count > 0)
                {
                    InsertBillItems(details.Items, con, id);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
        }
        public void InsertBillItems(List<Items> items, SqlConnection con, int id)
        {
            try
            {
                string qry = "Insert into tbl_BillItems(ProductName,Price,Quantity,BillId) values";
                foreach (var item in items)
                {
                    qry += String.Format("('{0}',{1},{2},{3}),", item.ProductName, item.Price, item.Quantity, id);
                }
                qry = qry.Remove(qry.Length - 1);
                SqlCommand cmd = new SqlCommand(qry, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BillDetail> GetAllDetails()
        {
            List<BillDetail> list = new List<BillDetail>();
            try
            {
                con.Open();
                cmd = new SqlCommand("spt_getAllBillDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    decimal totalAmount = decimal.Parse(dr["TotalAmount"].ToString());
                    string formattedTotalAmount = totalAmount.ToString("0.00");

                    BillDetail detail = new BillDetail
                    {
                        Id = int.Parse(dr["Id"].ToString()),
                        CustomerName = dr["CustomerName"].ToString(),
                        MobileNumber = dr["MobileNumber"].ToString(),
                        Address = dr["Address"].ToString(),
                        DateTime = DateTime.Parse(dr["DateTime"].ToString()),
                        TotalAmount = decimal.Parse(formattedTotalAmount)
                    };

                    list.Add(detail);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }
        public BillDetail GetDetail(int Id)
        {
            BillDetail detail = new BillDetail();
            Items item;
            try
            {
                con.Open();
                cmd = new SqlCommand("spt_getBillDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    decimal totalAmount = decimal.Parse(dr["TotalAmount"].ToString());
                    string formattedTotalAmount = totalAmount.ToString("0.00");
                    detail.Id = int.Parse(dr["BillId"].ToString());
                    detail.CustomerName = dr["CustomerName"].ToString();
                    detail.MobileNumber = dr["MobileNumber"].ToString();
                    detail.Address = dr["Address"].ToString();
                    detail.DateTime = DateTime.Parse(dr["DateTime"].ToString());
                    detail.TotalAmount = decimal.Parse(formattedTotalAmount);

                    decimal price = decimal.Parse(dr["price"].ToString());
                    string formattedprice = price.ToString("0.00");
                    item = new Items
                    {
                        Id = int.Parse(dr["ItemId"].ToString()),
                        ProductName = dr["ProductName"].ToString(),
                        Price = decimal.Parse(formattedprice),
                        Quantity = int.Parse(dr["Quantity"].ToString())
                    };
                    detail.Items.Add(item);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
            return detail;
        }

        public int GetLatestBillId()
        {
            int latestBillId = 0;
            try
            {
                con.Open();
                cmd = new SqlCommand("GetLatestBillId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    latestBillId = (int)result;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
            return latestBillId;
        }

    }
}