﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebChatDataData.Models.Context;

namespace WebChatData.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210422083955_BotsSync")]
    partial class BotsSync
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("BotChat", b =>
                {
                    b.Property<int>("BotsBotId")
                        .HasColumnType("integer");

                    b.Property<int>("ChatsChatID")
                        .HasColumnType("integer");

                    b.HasKey("BotsBotId", "ChatsChatID");

                    b.HasIndex("ChatsChatID");

                    b.ToTable("BotChat");
                });

            modelBuilder.Entity("ChatChatUser", b =>
                {
                    b.Property<int>("ChatsChatID")
                        .HasColumnType("integer");

                    b.Property<int>("MembersUserID")
                        .HasColumnType("integer");

                    b.HasKey("ChatsChatID", "MembersUserID");

                    b.HasIndex("MembersUserID");

                    b.ToTable("ChatChatUser");
                });

            modelBuilder.Entity("WebChatData.Models.Bot", b =>
                {
                    b.Property<int>("BotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("BotId");

                    b.ToTable("Bots");
                });

            modelBuilder.Entity("WebChatData.Models.Chat", b =>
                {
                    b.Property<int>("ChatID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("OwnerUserID")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("ChatID");

                    b.HasIndex("OwnerUserID");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("WebChatData.Models.ChatEvent", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ChatID")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("EventKey")
                        .HasColumnType("text");

                    b.HasKey("EventID");

                    b.HasIndex("ChatID");

                    b.ToTable("ChatEvents");
                });

            modelBuilder.Entity("WebChatData.Models.ChatUser", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("LoginName")
                        .HasColumnType("text");

                    b.Property<string>("NikName")
                        .HasColumnType("text");

                    b.HasKey("UserID");

                    b.ToTable("ChatUsers");
                });

            modelBuilder.Entity("WebChatData.Models.ChatUserEvent", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ChatID")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("EventKey")
                        .HasColumnType("text");

                    b.Property<int?>("UserID")
                        .HasColumnType("integer");

                    b.HasKey("EventID");

                    b.HasIndex("ChatID");

                    b.HasIndex("UserID");

                    b.ToTable("ChatUserEvents");
                });

            modelBuilder.Entity("WebChatData.Models.Message", b =>
                {
                    b.Property<int>("MessageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ChatID")
                        .HasColumnType("integer");

                    b.Property<int?>("FromUserID")
                        .HasColumnType("integer");

                    b.Property<bool>("IsEdited")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsReaded")
                        .HasColumnType("boolean");

                    b.Property<string>("MessageText")
                        .HasColumnType("text");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("MessageID");

                    b.HasIndex("ChatID");

                    b.HasIndex("FromUserID");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("WebChatData.Models.Synchronization", b =>
                {
                    b.Property<int>("SyncId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("SyncDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("SyncId");

                    b.ToTable("Synchronizations");
                });

            modelBuilder.Entity("BotChat", b =>
                {
                    b.HasOne("WebChatData.Models.Bot", null)
                        .WithMany()
                        .HasForeignKey("BotsBotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebChatData.Models.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsChatID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ChatChatUser", b =>
                {
                    b.HasOne("WebChatData.Models.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsChatID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebChatData.Models.ChatUser", null)
                        .WithMany()
                        .HasForeignKey("MembersUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebChatData.Models.Chat", b =>
                {
                    b.HasOne("WebChatData.Models.ChatUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerUserID");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("WebChatData.Models.ChatEvent", b =>
                {
                    b.HasOne("WebChatData.Models.Chat", null)
                        .WithMany("ChatEvents")
                        .HasForeignKey("ChatID");
                });

            modelBuilder.Entity("WebChatData.Models.ChatUserEvent", b =>
                {
                    b.HasOne("WebChatData.Models.Chat", null)
                        .WithMany("ChatUserEvents")
                        .HasForeignKey("ChatID");

                    b.HasOne("WebChatData.Models.ChatUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebChatData.Models.Message", b =>
                {
                    b.HasOne("WebChatData.Models.Chat", "Chat")
                        .WithMany("History")
                        .HasForeignKey("ChatID");

                    b.HasOne("WebChatData.Models.ChatUser", "From")
                        .WithMany()
                        .HasForeignKey("FromUserID");

                    b.Navigation("Chat");

                    b.Navigation("From");
                });

            modelBuilder.Entity("WebChatData.Models.Chat", b =>
                {
                    b.Navigation("ChatEvents");

                    b.Navigation("ChatUserEvents");

                    b.Navigation("History");
                });
#pragma warning restore 612, 618
        }
    }
}
