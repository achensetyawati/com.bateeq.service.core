using AutoMapper;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;

namespace Com.DanLiris.Service.Core.Lib.AutoMapperProfiles
{
    public class MachineSpinningProfile : Profile
    {
        public MachineSpinningProfile()
        {
            CreateMap<MachineSpinningModel, MachineSpinningViewModel>()
                .ReverseMap();
            CreateMap<MachineSpinningProcessType, MachineSpinningProcessTypeViewModel>()
                .ReverseMap();
        }
    }
}
