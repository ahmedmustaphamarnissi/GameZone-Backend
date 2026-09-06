using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GameZoneBack.Models;

public partial class GameMarketplaceContext : DbContext
{
    public GameMarketplaceContext()
    {
    }

    public GameMarketplaceContext(DbContextOptions<GameMarketplaceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CardType> CardTypes { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeesPicture> EmployeesPictures { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventGame> EventGames { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<FriendRequestStatus> FriendRequestStatuses { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameDevice> GameDevices { get; set; }

    public virtual DbSet<GameGenre> GameGenres { get; set; }

    public virtual DbSet<GameLanguage> GameLanguages { get; set; }

    public virtual DbSet<GamesFeature> GamesFeatures { get; set; }

    public virtual DbSet<GamesStatus> GamesStatuses { get; set; }

    public virtual DbSet<GamesType> GamesTypes { get; set; }

    public virtual DbSet<GamesVidsAndPicture> GamesVidsAndPictures { get; set; }

    public virtual DbSet<GlobalNotification> GlobalNotifications { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradePermission> GradePermissions { get; set; }

    public virtual DbSet<InstalledGame> InstalledGames { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PeopleStatus> PeopleStatuses { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<PurchasedGame> PurchasedGames { get; set; }

    public virtual DbSet<RequirementType> RequirementTypes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<SystemRequirement> SystemRequirements { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    public virtual DbSet<UsersPicture> UsersPictures { get; set; }

    public virtual DbSet<WishList> WishLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=;Database=Game_Marketplace;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardType>(entity =>
        {
            entity.HasKey(e => e.CardTypeId).HasName("PK__CardType__AB0A3D31393CD688");

            entity.Property(e => e.CardTypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__3214EC27ECE00C9E");

            entity.HasIndex(e => e.GameId, "Comments_GameID_IDX");

            entity.HasIndex(e => e.UserId, "Comments_UserID_IDX");

            entity.Property(e => e.CommentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CommentText).HasMaxLength(500);

            entity.HasOne(d => d.Game).WithMany(p => p.Comments)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__GameID__7B5B524B");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__UserID__7A672E12");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Companie__2D971C4C82CE1780");

            entity.HasIndex(e => e.CompanyName, "UQ__Companie__9BCE05DC49DC8FEB").IsUnique();

            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.LogoPath).HasMaxLength(255);
            entity.Property(e => e.Website).HasMaxLength(255);

            entity.HasOne(d => d.Country).WithMany(p => p.Companies)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_Companies_Countries");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Countrie__3214EC2752E6EFAA");

            entity.HasIndex(e => e.CountryCode, "UQ_Countries_CountryCode").IsUnique();

            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CountryName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneCode)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PK__Devices__3214EC278666F01F");

            entity.Property(e => e.DeviceName).HasMaxLength(50);
            entity.Property(e => e.IconPath).HasMaxLength(500);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1AB301833");

            entity.HasIndex(e => e.PersonId, "Employee_PersonID_IDX");

            entity.HasIndex(e => e.PictureId, "Employee_PictureID_IDX");

            entity.HasIndex(e => e.StatusId, "Employee_StatusID_IDX");

            entity.HasIndex(e => e.PersonId, "UQ_Employees_PersonId").IsUnique();

            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salary");

            entity.HasOne(d => d.Person).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__Person__6383C8BA");

            entity.HasOne(d => d.Picture).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PictureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__Pictur__6477ECF3");

            entity.HasOne(d => d.Status).WithMany(p => p.Employees)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Status");
        });

        modelBuilder.Entity<EmployeesPicture>(entity =>
        {
            entity.HasKey(e => e.PictureId).HasName("PK__Employee__3214EC27CC3D5543");

            entity.ToTable("Employees_Pictures");

            entity.Property(e => e.Path).HasMaxLength(500);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "IX_Event_StartDate_EndDate");

            entity.Property(e => e.BannerImagePath).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EventName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EventGame>(entity =>
        {
            entity.ToTable("EventGame");

            entity.HasIndex(e => e.GameId, "IX_EventGame_GameId");

            entity.HasIndex(e => new { e.EventId, e.GameId }, "UQ_EventGame_EventId_GameId").IsUnique();

            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Event).WithMany(p => p.EventGames)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventGame_Event");

            entity.HasOne(d => d.Game).WithMany(p => p.EventGames)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventGame_Games");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Features__3214EC079F0B733F");

            entity.HasIndex(e => e.FeatureName, "UQ__Features__55ABBB71A853BFDA").IsUnique();

            entity.Property(e => e.FeatureName).HasMaxLength(100);
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasIndex(e => e.ReceiverId, "IX_FriendRequests_ReceiverId");

            entity.HasIndex(e => e.SenderId, "IX_FriendRequests_SenderId");

            entity.HasIndex(e => e.StatusId, "IX_FriendRequests_StatusId");

            entity.HasIndex(e => new { e.SenderId, e.ReceiverId }, "UQ_FriendRequests_Sender_Receiver").IsUnique();

            entity.Property(e => e.RespondedDate).HasColumnType("datetime");
            entity.Property(e => e.SendDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Receiver).WithMany(p => p.FriendRequestReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.FriendRequestSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_Sender");

            entity.HasOne(d => d.Status).WithMany(p => p.FriendRequests)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_FriendRequestStatus");
        });

        modelBuilder.Entity<FriendRequestStatus>(entity =>
        {
            entity.ToTable("FriendRequestStatus");

            entity.HasIndex(e => e.StatusName, "UQ_FriendRequestStatus_StatusName").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__Games__2AB897DD58047F88");

            entity.HasIndex(e => e.CompanyId, "Games_CompanyID_IDX");

            entity.HasIndex(e => e.StatusId, "Games_StatusID_IDX");

            entity.Property(e => e.AdditionDate).HasColumnType("datetime");
            entity.Property(e => e.GameDescription).HasMaxLength(1000);
            entity.Property(e => e.GameName).HasMaxLength(100);
            entity.Property(e => e.GameVersion)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.InitialPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LastUpdate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.Games)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Games_Companies");

            entity.HasOne(d => d.Status).WithMany(p => p.Games)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Games_Status");
        });

        modelBuilder.Entity<GameDevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameDevi__3214EC27010CF189");

            entity.HasIndex(e => e.DeviceId, "GameDevices_DeviceID_IDX");

            entity.HasIndex(e => e.GameId, "GameDevices_GameID_IDX");

            entity.HasOne(d => d.Device).WithMany(p => p.GameDevices)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameDevic__Devic__0D7A0286");

            entity.HasOne(d => d.Game).WithMany(p => p.GameDevices)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameDevic__GameI__0E6E26BF");
        });

        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameGenr__3214EC07C4BE0079");

            entity.HasIndex(e => new { e.GameId, e.GenreId }, "UQ_GameGenres").IsUnique();

            entity.HasOne(d => d.Game).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameGenres_Games");

            entity.HasOne(d => d.Genre).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameGenres_GameTypes");
        });

        modelBuilder.Entity<GameLanguage>(entity =>
        {
            entity.HasIndex(e => e.GameId, "IX_GameLanguages_GameId");

            entity.HasIndex(e => e.LanguageId, "IX_GameLanguages_LanguageId");

            entity.HasIndex(e => new { e.GameId, e.LanguageId }, "UQ_GameLanguages_GameId_LanguageId").IsUnique();

            entity.HasOne(d => d.Game).WithMany(p => p.GameLanguages)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameLanguages_Games");

            entity.HasOne(d => d.Language).WithMany(p => p.GameLanguages)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameLanguages_Languages");
        });

        modelBuilder.Entity<GamesFeature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GamesFea__3214EC07C97B803A");

            entity.HasIndex(e => e.FeatureId, "IX_GamesFeatures_FeatureId");

            entity.HasIndex(e => e.GameId, "IX_GamesFeatures_GameId");

            entity.HasIndex(e => new { e.FeatureId, e.GameId }, "UQ_GamesFeatures").IsUnique();

            entity.HasOne(d => d.Feature).WithMany(p => p.GamesFeatures)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GamesFeatures_Features");

            entity.HasOne(d => d.Game).WithMany(p => p.GamesFeatures)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GamesFeatures_Games");
        });

        modelBuilder.Entity<GamesStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__GamesSta__C8EE204313C2EF93");

            entity.ToTable("GamesStatus");

            entity.Property(e => e.StatusDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GamesType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Games_Ty__3214EC2789F74764");

            entity.ToTable("Games_Types");

            entity.Property(e => e.TypeName).HasMaxLength(500);
        });

        modelBuilder.Entity<GamesVidsAndPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameVids__3214EC279D102FA1");

            entity.HasIndex(e => e.GameId, "GameVidsAndPictures_GameID_IDX");

            entity.Property(e => e.Path).HasMaxLength(1000);
            entity.Property(e => e.Type)
                .HasComment("0 picture 1 video")
                .HasColumnName("type");

            entity.HasOne(d => d.Game).WithMany(p => p.GamesVidsAndPictures)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameVidsA__GameI__08B54D69");
        });

        modelBuilder.Entity<GlobalNotification>(entity =>
        {
            entity.HasIndex(e => e.CreatedDate, "IX_GlobalNotifications_CreatedDate");

            entity.HasIndex(e => e.NotificationTypeId, "IX_GlobalNotifications_NotificationTypeId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByEmployee).WithMany(p => p.GlobalNotifications)
                .HasForeignKey(d => d.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GlobalNotifications_Employees");

            entity.HasOne(d => d.NotificationType).WithMany(p => p.GlobalNotifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GlobalNotifications_NotificationTypes");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A37F36766D3");

            entity.Property(e => e.GradeName).HasMaxLength(500);
        });

        modelBuilder.Entity<GradePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GradePer__3214EC2796616CBC");

            entity.HasIndex(e => e.GradeId, "GradePermissions_GradeID_IDX");

            entity.HasIndex(e => e.PermissionId, "GradePermissions_PermissionID_IDX");

            entity.HasOne(d => d.Grade).WithMany(p => p.GradePermissions)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GradePerm__Grade__5BE2A6F2");

            entity.HasOne(d => d.Permission).WithMany(p => p.GradePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GradePerm__Permi__5AEE82B9");
        });

        modelBuilder.Entity<InstalledGame>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Installe__3214EC07FB4E641C");

            entity.HasIndex(e => e.GameId, "IX_InstalledGames_GameId");

            entity.HasIndex(e => e.UserId, "IX_InstalledGames_UserId");

            entity.HasIndex(e => new { e.UserId, e.IsInstalled }, "IX_InstalledGames_User_Installed");

            entity.HasIndex(e => new { e.UserId, e.IsPaused }, "IX_InstalledGames_User_Paused");

            entity.HasIndex(e => new { e.UserId, e.GameId }, "UQ_InstalledGames_User_Game").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.InstallationVersion).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Game).WithMany(p => p.InstalledGames)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_InstalledGames_Game");

            entity.HasOne(d => d.User).WithMany(p => p.InstalledGames)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_InstalledGames_User");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasIndex(e => e.LanguageCode, "UQ_Languages_LanguageCode").IsUnique();

            entity.HasIndex(e => e.LanguageName, "UQ_Languages_LanguageName").IsUnique();

            entity.Property(e => e.LanguageCode).HasMaxLength(5);
            entity.Property(e => e.LanguageName).HasMaxLength(100);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasIndex(e => new { e.ReceiverUserId, e.SentAt }, "IX_Messages_ReceiverUserId_SentAt").IsDescending(false, true);

            entity.HasIndex(e => new { e.SenderUserId, e.SentAt }, "IX_Messages_SenderUserId_SentAt").IsDescending(false, true);

            entity.Property(e => e.Message1).HasColumnName("Message");
            entity.Property(e => e.ReadAt).HasColumnType("datetime");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ReceiverUser).WithMany(p => p.MessageReceiverUsers)
                .HasForeignKey(d => d.ReceiverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Receiver_Users");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.MessageSenderUsers)
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Sender_Users");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasIndex(e => e.NotificationTypeName, "UQ_NotificationTypes_NotificationTypeName").IsUnique();

            entity.Property(e => e.NotificationTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F34624A5CA");

            entity.HasIndex(e => e.CardTypeId, "PaymentMethods_CardTypeID_IDX");

            entity.HasIndex(e => e.PersonId, "PaymentMethods_PersonID_IDX");

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.CardHolder)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.CardTypeId).HasColumnName("CardTypeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");

            entity.HasOne(d => d.CardType).WithMany(p => p.PaymentMethods)
                .HasForeignKey(d => d.CardTypeId)
                .HasConstraintName("FK_PaymentMethods_CardType");

            entity.HasOne(d => d.Person).WithMany(p => p.PaymentMethods)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentMethods_Person");
        });

        modelBuilder.Entity<PeopleStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__PeopleSt__C8EE2043732D3631");

            entity.ToTable("PeopleStatus");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB0F50D9E6E3");

            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.PermissionName).HasMaxLength(500);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Person__AA2FFB85668F6003");

            entity.ToTable("Person");

            entity.HasIndex(e => e.CountryId, "Person_CountryID_IDX");

            entity.HasIndex(e => e.Email, "Person_Email_IDX");

            entity.HasIndex(e => e.GradeId, "Person_GradeID_IDX");

            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Country).WithMany(p => p.People)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Person__CountryI__398D8EEE");

            entity.HasOne(d => d.Grade).WithMany(p => p.People)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persons_Grade");
        });

        modelBuilder.Entity<PurchasedGame>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Purchase__3214EC27BB800F24");

            entity.HasIndex(e => e.GameId, "PurchasedGames_GameID_IDX");

            entity.HasIndex(e => e.PaymentMethodId, "PurchasedGames_PaymentMethodID_IDX");

            entity.HasIndex(e => e.UserId, "PurchasedGames_UserID_IDX");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PurchasedPrice).HasColumnType("smallmoney");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Game).WithMany(p => p.PurchasedGames)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__GameI__6FE99F9F");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.PurchasedGames)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK_PurchasedGames_PaymentMethod");

            entity.HasOne(d => d.User).WithMany(p => p.PurchasedGames)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchased__UserI__6EF57B66");
        });

        modelBuilder.Entity<RequirementType>(entity =>
        {
            entity.HasIndex(e => e.RequirementTypeName, "UQ_RequirementTypes_RequirementTypeName").IsUnique();

            entity.Property(e => e.RequirementTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE2A21A63B");

            entity.HasIndex(e => e.GameId, "Reviews_GameID_IDX");

            entity.HasIndex(e => e.UserId, "Reviews_UserID_IDX");

            entity.HasIndex(e => new { e.GameId, e.UserId }, "UQ_Reviews_GameId_UserId").IsUnique();

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.Review1).HasColumnName("Review");
            entity.Property(e => e.ReviewComment).HasMaxLength(1000);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Game).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_GameID");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_User");
        });

        modelBuilder.Entity<SystemRequirement>(entity =>
        {
            entity.HasIndex(e => e.DeviceId, "IX_SystemRequirements_DeviceId");

            entity.HasIndex(e => e.GameId, "IX_SystemRequirements_GameId");

            entity.HasIndex(e => e.RequirementTypeId, "IX_SystemRequirements_RequirementTypeId");

            entity.HasIndex(e => new { e.GameId, e.DeviceId, e.RequirementTypeId }, "UQ_SystemRequirements_Game_Device_Type").IsUnique();

            entity.Property(e => e.DirectX).HasMaxLength(100);
            entity.Property(e => e.Graphics).HasMaxLength(200);
            entity.Property(e => e.Memory).HasMaxLength(100);
            entity.Property(e => e.Network).HasMaxLength(200);
            entity.Property(e => e.OperatingSystem).HasMaxLength(200);
            entity.Property(e => e.Processor).HasMaxLength(200);
            entity.Property(e => e.SoundCard).HasMaxLength(200);
            entity.Property(e => e.Storage).HasMaxLength(100);

            entity.HasOne(d => d.Device).WithMany(p => p.SystemRequirements)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemRequirements_Devices");

            entity.HasOne(d => d.Game).WithMany(p => p.SystemRequirements)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemRequirements_Games");

            entity.HasOne(d => d.RequirementType).WithMany(p => p.SystemRequirements)
                .HasForeignKey(d => d.RequirementTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemRequirements_RequirementTypes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC1F3B5C72");

            entity.HasIndex(e => e.PersonId, "UQ_Users_PersonId").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F28456A57F791C").IsUnique();

            entity.HasIndex(e => e.PersonId, "Users_Person_ID_IDX");

            entity.HasIndex(e => e.PictureId, "Users_PictureID_IDX");

            entity.HasIndex(e => e.StatusId, "Users_StatusID_IDX");

            entity.HasIndex(e => e.UserName, "Users_UserName_IDX");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Bio)
                .HasMaxLength(100)
                .HasColumnName("bio");
            entity.Property(e => e.LastGlobalNotificationViewedDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.PictureId).HasColumnName("PictureID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Person).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__Person_ID__3F466844");

            entity.HasOne(d => d.Picture).WithMany(p => p.Users)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("FK__Users__PictureID__403A8C7D");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Status");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasIndex(e => e.CreatedDate, "IX_UserNotifications_CreatedDate");

            entity.HasIndex(e => e.IsRead, "IX_UserNotifications_IsRead");

            entity.HasIndex(e => e.NotificationTypeId, "IX_UserNotifications_NotificationTypeId");

            entity.HasIndex(e => e.UserId, "IX_UserNotifications_UserId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReadDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.NotificationType).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserNotifications_NotificationTypes");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserNotifications_Users");
        });

        modelBuilder.Entity<UsersPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users_Pi__3214EC273DBA807D");

            entity.ToTable("Users_Pictures");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Path).HasMaxLength(500);
        });

        modelBuilder.Entity<WishList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WishList__3214EC27162FA33C");

            entity.ToTable("WishList");

            entity.HasIndex(e => e.GameId, "WishList_GameID_IDX");

            entity.HasIndex(e => e.UserId, "WishList_UserID_IDX");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Game).WithMany(p => p.WishLists)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WishList__GameID__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.WishLists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WishList__UserID__76969D2E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
