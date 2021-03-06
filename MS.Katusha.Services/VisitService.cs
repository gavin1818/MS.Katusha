﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class VisitService : IVisitService
    {
        private readonly IVisitRepositoryDB _visitRepository;
        private readonly IVisitRepositoryRavenDB _visitRepositoryRaven;
        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;

        public VisitService(IVisitRepositoryDB visitRepository, IVisitRepositoryRavenDB visitRepositoryRaven, IProfileRepositoryRavenDB profileRepositoryRaven)
        {
            _visitRepository = visitRepository;
            _visitRepositoryRaven = visitRepositoryRaven;
            _profileRepositoryRaven = profileRepositoryRaven;
        }

        public void Visit(Profile visitorProfile, Profile profile)
        {
            if (visitorProfile != null && profile != null && profile.Id != visitorProfile.Id && profile.Guid != visitorProfile.Guid) {
                var visit = _visitRepository.SingleAttached(p => p.ProfileId == profile.Id && p.VisitorProfileId == visitorProfile.Id);

                if (visit == null) {
                    _visitRepository.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = 1 });
                } else {
                    visit.VisitCount++;
                    _visitRepository.FullUpdate(visit);
                }

                //TODO: Modify this with PATCH method of Raven Repository. http://ravendb.net/docs/client-api/partial-document-updates
                //_visitRepositoryRaven.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = visit.VisitCount });

                var visitRaven = _visitRepositoryRaven.SingleAttached(p => p.ProfileId == profile.Id && p.VisitorProfileId == visitorProfile.Id);
                if (visitRaven == null) {
                    _visitRepositoryRaven.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = 1 });
                } else {
                    visitRaven.VisitCount++;
                    _visitRepositoryRaven.FullUpdate(visitRaven);
                }
            }
        }

        public IList<UniqueVisitorsResult> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            var items = _visitRepositoryRaven.GetVisitors(profileId, out total, pageNo, pageSize);
            return items;
        }

        public IList<UniqueVisitorsResult> GetMyVisits(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            var items = _visitRepositoryRaven.GetMyVisits(profileId, out total, pageNo, pageSize);
            return items;
        }


        public NewVisits GetVisitorsSinceLastVisit(long profileId, DateTime lastVisitTime)
        {
            var lvt = lastVisitTime - new TimeSpan(0, 0, 5, 0);
            var visits = _visitRepositoryRaven.GetVisitorsSinceLastVisit(profileId, lvt);
            var newVisits = new NewVisits { LastVisitTime = lastVisitTime, Visits = visits };
            return newVisits;
        }

        public IList<string> RestoreFromDB(Expression<Func<Visit, bool>> filter, bool deleteIfExists = false)
        {
            var dbRepository = _visitRepository;
            var ravenRepository = _visitRepositoryRaven;

            var result = new List<string>();
            var items = dbRepository.Query(filter, null, false);
            foreach (var item in items) {
                try {
                    if (deleteIfExists) {
                        var p = ravenRepository.GetById(item.Id);
                        if (p != null)
                            ravenRepository.Delete(p);
                    }
                    ravenRepository.Add(item);
                } catch (Exception ex) {
                    result.Add(String.Format("{0} - {1}", item.Id, ex.Message));
                }
            }
            return result;
        }
    }
}
