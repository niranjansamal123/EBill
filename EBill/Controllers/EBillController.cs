using EBill.Models;
using System.Web.Mvc;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace EBill.Controllers
{
    public class EBillController : Controller
    {
        BillDAL dal = new BillDAL();
        public ViewResult DisplayBills()
        {
            var list = dal.GetAllDetails();
            return View(list);
        }
        public ViewResult DisplayBill(int Id)
        {
            //var billdetails = dal.GetAllDetails().OrderByDescending(x => x.Id).FirstOrDefault();
            var billdetails = dal.GetDetail(Id);
            return View(billdetails);
        }
        [HttpGet]
        public ViewResult CreateBill()
        {
            return View();
        }
        [HttpPost]
        public RedirectToRouteResult CreateBill(BillDetail details)
        {
            dal.InsertBillDetails(details);
            ModelState.Clear();
            return RedirectToAction("DisplayBill");
            
        }

        public ActionResult CreateItem(Items item)
        {
            return PartialView("_CreateItem", item);
        }

    }
}