﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Models
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.StartDate, opt => opt.MapFrom(camp => camp.EventDate))
                .ForMember(c => c.EndDate,   opt => opt.ResolveUsing(camp => camp.EventDate.AddDays(camp.Length-1)))
                .ForMember(c => c.Url,       opt => opt.ResolveUsing<CampUrlResolver>())
                .ReverseMap()
                .ForMember(m => m.EventDate, opt => opt.MapFrom(model => model.StartDate))
                .ForMember(m => m.Length,    opt => opt.ResolveUsing(model => (model.EndDate- model.StartDate).Days+1))
                .ForMember(m => m.Location,  opt => opt.ResolveUsing(model => new Location()
                {
                    Address1 = model.LocationAddress1,
                    Address2 = model.LocationAddress2,
                    Address3 = model.LocationAddress3,
                    CityTown = model.LocationCityTown,
                    Country =  model.LocationCountry,
                    PostalCode = model.LocationPostalCode,
                    StateProvince = model.LocationStateProvince
                }));
            CreateMap<Speaker, SpeakerModel>()
                .ForMember(s => s.Url,opt => opt.ResolveUsing<SpeakerUrlResolver>())
                .ReverseMap();

           CreateMap<Talk, TalkModel>()
             .ForMember(s => s.Url, opt => opt.ResolveUsing<TalkUrlResolver>())
             .ReverseMap();
        }
    }
}
