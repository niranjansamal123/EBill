using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EBill.Models
{
    public class BillDetail
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Customer Name is required.")]
        [RegularExpression(@"[A-Za-z\s]{3,}", ErrorMessage = "Name can have alphabets & spaces with min size of 3.")]
        public string CustomerName { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"[6-9]\d{9}", ErrorMessage = "Mobile No. should start with 6, 7, 8, and 9 only and can be having a length of 10 digits(Both Max & Min).")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Items> Items { get; set; }
        public BillDetail()
        {
            Items = new List<Items>();
        }
    }
}