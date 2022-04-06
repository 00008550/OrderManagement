using Dapper;
using OrderManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace OrderManagement.DAL
{
    public class OrderRepository : IOrderRepository
    {
        private const string SELECT = @"SELECT [OrderId]
      ,[StoreName]
      ,[ManagerOrderId]
      ,[CustomerOrderId]
      ,[SellerOrderId]
      ,[ItemOrderId]
      ,[Price]
      ,[DateOfOrder]
      ,[ImageUrl]
      ,[ItemQuantity]
      ,[ItemQuality]
,[Delivered]
  FROM [dbo].[Order]";
        private const string FILTER = @"SELECT {0}
                                        FROM [dbo].[Order]
                                        {1} {2}";

        private const string INSERT = @"INSERT INTO [dbo].[Order]
           ([StoreName]
           ,[ManagerOrderId]
           ,[CustomerOrderId]
           ,[SellerOrderId]
           ,[ItemOrderId]
           ,[Price]
           ,[DateOfOrder]
           ,[ImageUrl]
           ,[ItemQuantity]
           ,[ItemQuality])
     VALUES
           (@StoreName
           ,@ManagerOrderId
           ,@CustomerOrderId
           ,@SellerOrderId
           ,@ItemOrderId
           ,@Price
           ,@DateOfOrder
           ,@ImageUrl
           ,@ItemQuantity
           ,@ItemQuality)";
        private const string DELETE = @"delete 
                                        from [dbo].[Order] 
                                        where OrderId = @OrderId";
        private const string UNLINK_FK = @"update [dbo].[Order] 
                                                   set ManagerOrder = null,
                                                        CustomerOrderId = null,
                                                        SellerOrderId = null,
                                                        ItemOrderId = null
                                                   where OrderId = @OrderId";

        private string connStr;

        public OrderRepository(string connStr)
        {
            this.connStr = connStr;
        }

        public Order GetByIdSP(int id)
        {
            Order order = new Order();
            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "udpGetOrderById";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@OrderId", System.Data.SqlDbType.NVarChar, 20).Value = id;
                    //cmd.Parameters.AddWithValue("@OrderId", order.OrderId);

                    conn.Open();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            order = new Order();
                            order.OrderId = id;

                            order.StoreName = rdr.GetString(rdr.GetOrdinal("StoreName"));
                            order.ManagerOrderId = rdr.GetInt32(rdr.GetOrdinal("ManagerOrderId"));
                            order.CustomerOrderId = rdr.GetInt32(rdr.GetOrdinal("CustomerOrderId"));
                            order.SellerOrderId = rdr.GetInt32(rdr.GetOrdinal("SellerOrderId"));
                            order.ItemOrderId = rdr.GetInt32(rdr.GetOrdinal("ItemOrderId"));
                            order.Price = rdr.GetDecimal(rdr.GetOrdinal("Price"));
                            order.DateOfOrder = rdr.GetDateTime(rdr.GetOrdinal("DateOfOrder"));
                            order.ImageUrl = rdr.GetString(rdr.GetOrdinal("ImageUrl"));
                            order.ItemQuantity = rdr.GetInt32(rdr.GetOrdinal("ItemQuantity"));
                            order.ItemQuality = rdr.GetString(rdr.GetOrdinal("ItemQuality"));
                            order.Delivered = rdr.GetBoolean(rdr.GetOrdinal("Delivered"));
                        }

                    }
                }
            }

            return order;
        }

        public Order GetByIdSP_out(int id)
        {
            Order order = new Order();
            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "udpGetOrderById_out";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@OrderId", System.Data.SqlDbType.NVarChar, 20).Value = id;
                    //cmd.Parameters.AddWithValue("@OrderId", order.OrderId);

                    cmd.Parameters.AddWithValue("@EId", id);

                    var pStoreName = cmd.Parameters.Add("@StoreName", SqlDbType.NVarChar, 200);
                    pStoreName.Direction = ParameterDirection.Output;

                    var pManagerId = cmd.Parameters.Add("@ManagerOrderId", SqlDbType.Int);
                    pManagerId.Direction = ParameterDirection.Output;

                    var pCustomerId = cmd.Parameters.Add("@CustomerOrderId", SqlDbType.Int);
                    pCustomerId.Direction = ParameterDirection.Output;

                    var pSellerId = cmd.Parameters.Add("@SellerOrderId", SqlDbType.Int);
                    pSellerId.Direction = ParameterDirection.Output;

                    var pItemId = cmd.Parameters.Add("@ItemOrderId", SqlDbType.Int);
                    pItemId.Direction = ParameterDirection.Output;
                    var pPrice = cmd.Parameters.Add("@Price", SqlDbType.Decimal);
                    pPrice.Direction = ParameterDirection.Output;
                    var pDate = cmd.Parameters.Add("@DateOfOrder", SqlDbType.DateTime);
                    pDate.Direction = ParameterDirection.Output;
                    var pImage = cmd.Parameters.Add("@ImageUrl", SqlDbType.Int);
                    pImage.Direction = ParameterDirection.Output;
                    var pItemQuantity = cmd.Parameters.Add("@ItemQuantity", SqlDbType.Int);
                    pItemQuantity.Direction = ParameterDirection.Output;
                    var pItemQuality = cmd.Parameters.Add("@ItemOrderId", SqlDbType.Int);
                    pItemQuality.Direction = ParameterDirection.Output;

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    order.StoreName = (string)pStoreName.Value;
                    order.ManagerOrderId = (int)pManagerId.Value;
                    //order.CustomerOrderId = (int)pCustomerId.Value;
                    //order.SellerOrderId = (int)pSellerId.Value;
                    //order.ItemOrderId = (int)pItemId.Value;
                    //order.Price = (int)pPrice.Value;
                    //order.DateOfOrder = (DateTime)pDate.Value;
                    //order.ImageUrl = (string)pImage.Value;
                    //order.ItemQuantity = (int)pItemQuantity.Value;
                    //order.ItemQuality = (string)pItemQuality.Value;
                }
            }

            return order;
        }

        private List<Order> GetOrders()
        {
            var list = new List<Order>();

            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SELECT;
                    conn.Open();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var order = MapReaderToOrders(rdr);

                            list.Add(order);
                        }
                    }
                }

            }
            return list;
        }

        public void Insert(Order order)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("udpAddOrder", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@StoreName", order.StoreName);
                cmd.Parameters.AddWithValue("@ManagerOrderId", order.ManagerOrderId);
                cmd.Parameters.AddWithValue("@CustomerOrderId", order.CustomerOrderId);
                cmd.Parameters.AddWithValue("@SellerOrderId", order.SellerOrderId);
                cmd.Parameters.AddWithValue("@ItemOrderId", order.ItemOrderId);
                cmd.Parameters.AddWithValue("@Price", order.Price);
                cmd.Parameters.AddWithValue("@DateOfOrder", order.DateOfOrder);
                cmd.Parameters.AddWithValue("@ImageUrl", order.ImageUrl);
                cmd.Parameters.AddWithValue("@ItemQuantity", order.ItemQuantity);
                cmd.Parameters.AddWithValue("@ItemQuality", order.ItemQuality);
                cmd.Parameters.AddWithValue("@Delivered", order.Delivered);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void UpdateSP(Order order)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("udpUpdate", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@OrderId", order.OrderId);
                cmd.Parameters.AddWithValue("@StoreName", order.StoreName);
                cmd.Parameters.AddWithValue("@ManagerOrderId", order.ManagerOrderId);
                cmd.Parameters.AddWithValue("@CustomerOrderId", order.CustomerOrderId);
                cmd.Parameters.AddWithValue("@SellerOrderId", order.SellerOrderId);
                cmd.Parameters.AddWithValue("@ItemOrderId", order.ItemOrderId);
                cmd.Parameters.AddWithValue("@Price", order.Price);
                cmd.Parameters.AddWithValue("@DateOfOrder", order.DateOfOrder);
                cmd.Parameters.AddWithValue("@ImageUrl", order.ImageUrl);
                cmd.Parameters.AddWithValue("@ItemQuantity", order.ItemQuantity);
                cmd.Parameters.AddWithValue("@ItemQuality", order.ItemQuality);
                cmd.Parameters.AddWithValue("@Delivered", order.Delivered);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("udpDelete", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderId", id);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private Order MapReaderToOrders(DbDataReader rdr)
        {
            Order order = new Order();
            order.OrderId = rdr.GetInt32(rdr.GetOrdinal("OrderId"));
            order.StoreName = rdr.GetString(rdr.GetOrdinal("StoreName"));
            order.ManagerOrderId = rdr.GetInt32(rdr.GetOrdinal("ManagerOrderId"));
            order.CustomerOrderId = rdr.GetInt32(rdr.GetOrdinal("CustomerOrderId"));
            order.SellerOrderId = rdr.GetInt32(rdr.GetOrdinal("SellerOrderId"));
            order.ItemOrderId = rdr.GetInt32(rdr.GetOrdinal("ItemOrderId"));
            order.Price = rdr.GetDecimal(rdr.GetOrdinal("Price"));
            order.DateOfOrder = rdr.GetDateTime(rdr.GetOrdinal("DateOfOrder"));
            order.ImageUrl = rdr.GetString(rdr.GetOrdinal("ImageUrl"));
            order.ItemQuantity = rdr.GetInt32(rdr.GetOrdinal("ItemQuantity"));
            order.ItemQuality = rdr.GetString(rdr.GetOrdinal("ItemQuality"));
            order.Delivered = rdr.GetBoolean(rdr.GetOrdinal("Delivered"));

            return order;
        }
        private List<Order> GetOrdersDapper()
        {
            using (var conn = new SqlConnection(connStr))
                return conn.Query<Order>(SELECT)
                           .AsList();

        }

        List<Order> IOrderRepository.GetOrders()
        {
            return GetOrdersDapper();
        }

        public List<Order> Filter(
            string StoreName,
                int ManagerOrderId,
                int CustomerOrderId,
                int SellerOrderId,
                int ItemOrderId,
                out int totalCount,
                string sortColumn,
                bool sortDesc = false,
                int? page = 1,
                int pageSize = 2)
        {
            using (var conn = new SqlConnection(connStr))
            {
                var parameters = new DynamicParameters();

                #region Filtration
                string sqlWhere = "";

                if (!string.IsNullOrWhiteSpace(StoreName))
                {
                    sqlWhere += " StoreName like  '%' + rtrim(ltrim(@StoreName)) + '%' AND";
                    parameters.Add("@StoreName", StoreName);
                }
                if (ManagerOrderId != 0)
                {
                    sqlWhere += " ManagerOrderId like  '%' + rtrim(ltrim(@ManagerOrderId)) + '%' AND";
                    parameters.Add("@ManagerOrderId", ManagerOrderId);
                }
                if (CustomerOrderId != 0)
                {
                    sqlWhere += " CustomerOrderId like  '%' + rtrim(ltrim(@CustomerOrderId)) + '%' AND";
                    parameters.Add("@CustomerOrderId", CustomerOrderId);
                }
                if (SellerOrderId != 0)
                {
                    sqlWhere += " SellerOrderId like  '%' + rtrim(ltrim(@SellerOrderId)) + '%' AND";
                    parameters.Add("@SellerOrderId", SellerOrderId);
                }

                if (ItemOrderId != 0)
                {
                    sqlWhere += " ItemOrderId like rtrim(ltrim(@ItemOrderId)) + '%' AND";
                    parameters.Add("@ItemOrderId", ItemOrderId);
                }


                if (sqlWhere.Length > 0)
                    sqlWhere = " WHERE " + sqlWhere.Substring(0, sqlWhere.Length - 3);
                #endregion Filtration

                #region Sorting
                string orderByColumn = "StoreName";
                if (sortColumn != null)
                {
                    if (sortColumn.Equals("StoreName", StringComparison.OrdinalIgnoreCase))
                    {
                        orderByColumn = "StoreName";
                    }
                    else if (sortColumn.Equals("ManagerOrderId", StringComparison.OrdinalIgnoreCase))
                    {
                        orderByColumn = "ManagerOrderId";
                    }
                    else if (sortColumn.Equals("CustomerOrderId", StringComparison.OrdinalIgnoreCase))
                    {
                        orderByColumn = "CustomerOrderId";
                    }
                    else if (sortColumn.Equals("SellerOrderId", StringComparison.OrdinalIgnoreCase))
                    {
                        orderByColumn = "SellerOrderId";
                    }
                    else if (sortColumn.Equals("ItemOrderId", StringComparison.OrdinalIgnoreCase))
                    {
                        orderByColumn = "ItemOrderId";
                    }
                }

                string sqlSorting = $" ORDER BY {orderByColumn} {(sortDesc ? "DESC" : "ASC")} ";
                #endregion Sorting

                #region Paging
                if (!page.HasValue || page <= 0)
                    page = 1;

                if (pageSize <= 0)
                    pageSize = 2;

                totalCount = conn.ExecuteScalar<int>(
                        string.Format(FILTER, " count(*) ", sqlWhere, ""),
                        parameters
                    );

                string sqlPaging = @$"{sqlSorting}
                                     offset @Rows rows fetch next @PageSize rows only";

                parameters.Add("@Rows", (page - 1) * pageSize);
                parameters.Add("@PageSize", pageSize);
                #endregion Paging

                string sqlSelectList = @"[OrderId]
                                        ,[StoreName]
                                        ,[ManagerOrderId]
                                        ,[CustomerOrderId]                                                   
                                        ,[SellerOrderId]     
                                        ,[ItemOrderId]     
                                        ,[Price]     
                                        ,[DateOfOrder]     
                                        ,[ImageUrl]     
                                        ,[ItemQuantity]     
                                        ,[ItemQuality]
                                        ,[Delivered]";
                string sql = string.Format(FILTER, sqlSelectList, sqlWhere, sqlPaging);

                return conn.Query<Order>(sql, parameters).AsList();
            }
        }

    }
}
    
