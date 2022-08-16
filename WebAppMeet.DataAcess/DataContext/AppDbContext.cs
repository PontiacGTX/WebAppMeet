using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;

namespace WebAppMeet.DataAcess.DataContext
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts):base(opts)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Meeting>().HasOne(x => x.Host).WithMany(x => x.HostedMeetings)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserMeetings>().HasOne(x => x.User).WithMany(x => x.InvitedMeetings)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserMeetings>().HasOne(x => x.Meeting).WithMany(x => x.Invitees).HasForeignKey(x=>x.MeetingId)
               .OnDelete(DeleteBehavior.Restrict); 
            builder.Entity<ChatLog>().HasOne(x => x.Meeting).WithMany(x => x.ChatLogs)
               .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
            
        }
    }
}
