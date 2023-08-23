using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    public static class ObjectMapper
    {
        public static readonly Lazy<IMapper> lazy = new(() =>
        {
            var configuration = new MapperConfiguration(options =>
            {
                options.AddProfile<DtoMapper>();
            });

            return configuration.CreateMapper();
        });

        public static IMapper Mapper => lazy.Value;
    }
}
