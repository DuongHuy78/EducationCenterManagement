using AutoMapper;
using QuanLyTrungTamDaoTao.Models;
using QuanLyTrungTamDaoTao.ViewModels;

namespace QuanLyTrungTamDaoTao.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Map from RegisterVM(ViewModel) to HocVien(Data Entity)
            // AutoMapper maps properties with the same name by convention(e.g., HoTen)
            CreateMap<RegisterVM, HocVien>()
                .ReverseMap(); // Allows mapping from HocVien back to RegisterVM if needed
        }
    }
}
