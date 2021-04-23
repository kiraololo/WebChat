using Microsoft.EntityFrameworkCore;
using WebChatData.Models;

namespace WebChatDataData.Models.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(entity=> {
                entity.HasMany(c => c.Members).WithMany(m => m.Chats);
                entity.HasOne(c => c.Owner);
                entity.HasMany(c => c.Bots).WithMany(b => b.Chats);
            });
            modelBuilder.Entity<Message>(entity => {
                entity.HasOne(m => m.Chat);
            });            
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatEvent> ChatEvents { get; set; }
        public DbSet<ChatUserEvent> ChatUserEvents { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Bot> Bots { get; set; }
        public DbSet<Synchronization> Synchronizations { get; set; }
        public DbSet<AngryBotDictionary> AngryBotDictionary { get; set; }
    }
}
