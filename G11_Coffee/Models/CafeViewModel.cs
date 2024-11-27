using System.ComponentModel.DataAnnotations;

namespace G11_Coffee.Models
{
    public class CafeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên chi nhánh là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên chi nhánh không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; }

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string Phone { get; set; }

        public string Image { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile ImageFile { get; set; }
    }
}