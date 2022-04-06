using Dapper;
using OrderManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagement.DAL
{
    public interface IOrderRepository
    {
        public List<Order> GetOrders();

        public void Insert(Order order);

        public void UpdateSP(Order order);

        public Order GetByIdSP(int id);
        public Order GetByIdSP_out(int id);

        public void Delete(int id);
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
                int pageSize = 2
            );
    }
}
