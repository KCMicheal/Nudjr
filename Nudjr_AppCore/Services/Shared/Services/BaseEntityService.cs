using AutoMapper;
using Nudjr_Persistence.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public abstract class BaseEntityService
    {

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public BaseEntityService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

    }
}
