using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace C300.Models
{
    public partial class AppDbContext : DbContext
    {
        public virtual DbSet<Backlog> Backlog { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Dash> Dash { get; set; }
        public virtual DbSet<Emsuser> Emsuser { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }
        public virtual DbSet<FeedbackType> FeedbackType { get; set; }
        public virtual DbSet<Humidity> Humidity { get; set; }
        public virtual DbSet<Light> Light { get; set; }
        public virtual DbSet<Preference> Preference { get; set; }
        public virtual DbSet<Temperature> Temperature { get; set; }
        public virtual DbSet<Thread> Thread { get; set; }
        public virtual DbSet<Topic> Topic { get; set; }
        public virtual DbSet<Weight> Weight { get; set; }
        public virtual DbSet<Words> Words { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Backlog>(entity =>
            {
                entity.Property(e => e.BacklogId).HasColumnName("Backlog_id");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(30)");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("Comment_id");

                entity.Property(e => e.Anonymous).HasColumnName("anonymous");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("Created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Like)
                    .HasColumnName("like")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Picture).HasColumnType("varchar(max)");

                entity.Property(e => e.Report).HasDefaultValueSql("0");

                entity.Property(e => e.ThreadId).HasColumnName("Thread_id");

                entity.HasOne(d => d.Thread)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.ThreadId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_comment_forum");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("PK__Contact__A9D1053503F01971");

                entity.Property(e => e.Email).HasColumnType("varchar(100)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(max)");
            });

            modelBuilder.Entity<Dash>(entity =>
            {
                entity.HasKey(e => e.DId)
                    .HasName("PK__tmp_ms_x__76B7E3751425B14C");

                entity.Property(e => e.DId).HasColumnName("D_id");

                entity.Property(e => e.Ah1).HasColumnName("AH1");

                entity.Property(e => e.Ah2).HasColumnName("AH2");

                entity.Property(e => e.Ah3).HasColumnName("AH3");

                entity.Property(e => e.Ah4).HasColumnName("AH4");

                entity.Property(e => e.Ah5).HasColumnName("AH5");

                entity.Property(e => e.La).HasColumnName("LA");

                entity.Property(e => e.Lf).HasColumnName("LF");

                entity.Property(e => e.Lh).HasColumnName("LH");

                entity.Property(e => e.Ll).HasColumnName("LL");

                entity.Property(e => e.Lm).HasColumnName("LM");

                entity.Property(e => e.Lt).HasColumnName("LT");

                entity.Property(e => e.Lw).HasColumnName("LW");

                entity.Property(e => e.Ohh).HasColumnName("OHH");

                entity.Property(e => e.Ohl).HasColumnName("OHL");

                entity.Property(e => e.Olh).HasColumnName("OLH");

                entity.Property(e => e.Oll).HasColumnName("OLL");

                entity.Property(e => e.Oth).HasColumnName("OTH");

                entity.Property(e => e.Otl).HasColumnName("OTL");

                entity.Property(e => e.Owh).HasColumnName("OWH");

                entity.Property(e => e.Owl).HasColumnName("OWL");

                entity.Property(e => e.UserId).HasColumnName("User_id");
            });

            modelBuilder.Entity<Emsuser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Emsuser__1788CC4C0DD75BDB");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.Dob).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Picture).HasColumnType("varchar(max)");

                entity.Property(e => e.ResetPasswordCode).HasMaxLength(100);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Status).HasColumnType("varchar(100)");

                entity.Property(e => e.UpdatedBy).HasColumnType("nchar(150)");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.Description).HasColumnType("varchar(max)");

                entity.Property(e => e.EndDay).HasColumnType("datetime");

                entity.Property(e => e.IsFullDay).HasColumnName("isFullDay");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Subject).HasColumnType("varchar(200)");

                entity.Property(e => e.ThemeColor).HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Datetime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(1000)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.FeedbackType).HasColumnName("Feedback_type");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Reply).HasColumnType("varchar(1000)");

                entity.Property(e => e.SolvedTime).HasColumnType("datetime");

                entity.Property(e => e.Solvedby).HasColumnType("varchar(50)");

                entity.Property(e => e.Subject).HasColumnType("varchar(300)");
            });

            modelBuilder.Entity<FeedbackType>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<Humidity>(entity =>
            {
                entity.Property(e => e.HumidityId).HasColumnName("Humidity_id");

                entity.Property(e => e.HDatetime)
                    .HasColumnName("H_datetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.HLevel).HasColumnName("H_level");

                entity.Property(e => e.HrDatetime)
                    .HasColumnName("HR_datetime")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Light>(entity =>
            {
                entity.Property(e => e.LightId).HasColumnName("Light_id");

                entity.Property(e => e.LDatetime)
                    .HasColumnName("L_datetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LLevel).HasColumnName("L_level");

                entity.Property(e => e.LrDatetime)
                    .HasColumnName("LR_datetime")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Preference>(entity =>
            {
                entity.Property(e => e.PreferenceId).HasColumnName("Preference_id");

                entity.Property(e => e.HighestHumidity).HasColumnName("Highest_humidity");

                entity.Property(e => e.HighestLight).HasColumnName("Highest_light");

                entity.Property(e => e.HighestTemp).HasColumnName("Highest_temp");

                entity.Property(e => e.HighestWeight).HasColumnName("Highest_weight");

                entity.Property(e => e.HumNoti).HasColumnName("Hum_noti");

                entity.Property(e => e.HumUnit).HasColumnName("Hum_unit");

                entity.Property(e => e.LightNoti).HasColumnName("Light_noti");

                entity.Property(e => e.LightUnit).HasColumnName("Light_unit");

                entity.Property(e => e.LowestHumidity).HasColumnName("Lowest_humidity");

                entity.Property(e => e.LowestLight).HasColumnName("Lowest_light");

                entity.Property(e => e.LowestTemp).HasColumnName("Lowest_temp");

                entity.Property(e => e.LowestWeight).HasColumnName("Lowest_weight");

                entity.Property(e => e.TempNoti).HasColumnName("Temp_noti");

                entity.Property(e => e.TempUnit).HasColumnName("Temp_unit");

                entity.Property(e => e.WeightNoti).HasColumnName("Weight_noti");

                entity.Property(e => e.WeightUnit).HasColumnName("Weight_unit");
            });

            modelBuilder.Entity<Temperature>(entity =>
            {
                entity.Property(e => e.TemperatureId).HasColumnName("Temperature_id");

                entity.Property(e => e.TDatetime)
                    .HasColumnName("T_datetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.TLevel).HasColumnName("T_level");

                entity.Property(e => e.TrDatetime)
                    .HasColumnName("TR_datetime")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Thread>(entity =>
            {
                entity.Property(e => e.ThreadId).HasColumnName("Thread_id");

                entity.Property(e => e.CommentCount).HasDefaultValueSql("0");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("Created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Like)
                    .HasColumnName("like")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ThreadDescription)
                    .IsRequired()
                    .HasColumnName("Thread_Description")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.ThreadName)
                    .IsRequired()
                    .HasColumnName("Thread_name")
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.TopicName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UserId).HasDefaultValueSql("100");
            });

            modelBuilder.Entity<Weight>(entity =>
            {
                entity.Property(e => e.WeightId).HasColumnName("Weight_id");

                entity.Property(e => e.WDatetime)
                    .HasColumnName("W_datetime")
                    .HasColumnType("datetime");

                entity.Property(e => e.WLevel).HasColumnName("W_level");

                entity.Property(e => e.WrDatetime)
                    .HasColumnName("WR_datetime")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Words>(entity =>
            {
                entity.HasKey(e => e.Wordid)
                    .HasName("PK__words__F8094EE50E046CBA");

                entity.ToTable("words");

                entity.Property(e => e.Wordid).HasColumnName("wordid");

                entity.Property(e => e.Word)
                    .IsRequired()
                    .HasColumnName("word")
                    .HasColumnType("varchar(200)");
            });
        }
    }
}