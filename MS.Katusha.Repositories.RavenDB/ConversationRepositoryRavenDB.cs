using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class ConversationRepositoryRavenDB : BaseGuidRepositoryRavenDB<Conversation>, IConversationRepositoryRavenDB
    {
        public ConversationRepositoryRavenDB(IKatushaRavenStore documentStore)
            : base(documentStore) { }


        public IList<ConversationResult> MyConversations(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var query = session.Query<ConversationResult, ConversationIndex>().Statistics(out stats).Where(p=> p.ToId == profileId || p.FromId == profileId).Skip((pageNo - 1) * pageSize).Take( pageSize ).ToList();
                total = stats.TotalResults;
                return query;
                //.AsProjection<Profile>()
            }
        }

        public ConversationCountResult GetConversationStatistics(long profileId, MessageType messageType = MessageType.Received)
        {
            using (var session = DocumentStore.OpenSession()) {
                if(messageType == MessageType.Received)
                 return session.Query<ConversationCountResult, ConversationToCountIndex>().FirstOrDefault(p => p.ProfileId == profileId);
                return session.Query<ConversationCountResult, ConversationFromCountIndex>().FirstOrDefault(p => p.ProfileId == profileId);
            }
        }
    }
}