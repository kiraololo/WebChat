﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebChat.Models.Context;

namespace WebChat.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210416132910_EventsWithDate")]
    partial class EventsWithDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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

            modelBuilder.Entity("WebChat.Models.Chat", b =>
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

            modelBuilder.Entity("WebChat.Models.ChatEvent", b =>
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

            modelBuilder.Entity("WebChat.Models.ChatUser", b =>
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

            modelBuilder.Entity("WebChat.Models.ChatUserEvent", b =>
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

            modelBuilder.Entity("WebChat.Models.Message", b =>
                {
                    b.Property<int>("MessageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ChatID")
                        .HasColumnType("integer");

                    b.Property<int?>("FromUserID")
                        .HasColumnType("integer");

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

            modelBuilder.Entity("ChatChatUser", b =>
                {
                    b.HasOne("WebChat.Models.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsChatID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebChat.Models.ChatUser", null)
                        .WithMany()
                        .HasForeignKey("MembersUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebChat.Models.Chat", b =>
                {
                    b.HasOne("WebChat.Models.ChatUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerUserID");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("WebChat.Models.ChatEvent", b =>
                {
                    b.HasOne("WebChat.Models.Chat", null)
                        .WithMany("ChatEvents")
                        .HasForeignKey("ChatID");
                });

            modelBuilder.Entity("WebChat.Models.ChatUserEvent", b =>
                {
                    b.HasOne("WebChat.Models.Chat", null)
                        .WithMany("ChatUserEvents")
                        .HasForeignKey("ChatID");

                    b.HasOne("WebChat.Models.ChatUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebChat.Models.Message", b =>
                {
                    b.HasOne("WebChat.Models.Chat", null)
                        .WithMany("History")
                        .HasForeignKey("ChatID");

                    b.HasOne("WebChat.Models.ChatUser", "From")
                        .WithMany()
                        .HasForeignKey("FromUserID");

                    b.Navigation("From");
                });

            modelBuilder.Entity("WebChat.Models.Chat", b =>
                {
                    b.Navigation("ChatEvents");

                    b.Navigation("ChatUserEvents");

                    b.Navigation("History");
                });
#pragma warning restore 612, 618
        }
    }
}
