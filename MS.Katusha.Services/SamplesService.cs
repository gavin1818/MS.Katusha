﻿using System.Collections.Generic;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Services.Generators;

namespace MS.Katusha.Services
{
    public class SamplesService : ISamplesService
    {
        private readonly SampleGenerator _generator;

        public SamplesService(IUserService userService, IProfileService profileService, IPhotosService photosService, IConversationService conversationService, IVisitService visitService)
        {
            _generator = new SampleGenerator(profileService, userService, photosService, conversationService, visitService);
        }

        public void GenerateRandomUserAndProfile(int count, int extra = 0)
        {
            _generator.CreateSamples(count, extra);
        }

        public void GenerateRandomConversation(int count, int extra = 0)
        {
            _generator.CreateConversations(count, extra);
        }

        public void GenerateRandomVisit(int count, int extra = 0) { _generator.CreateVisit(count, extra); }
    }
}