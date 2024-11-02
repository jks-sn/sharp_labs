﻿// <auto-generated />
using Hackathon.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hackathon.Migrations
{
    [DbContext(typeof(HackathonDbContext))]
    [Migration("20241102140828_MakeAssignedPartnerNonNullable")]
    partial class MakeAssignedPartnerNonNullable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Hackathon.Model.HackathonEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Harmonic")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Hackathons");
                });

            modelBuilder.Entity("Hackathon.Model.Participant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AssignedPartner")
                        .HasColumnType("text");

                    b.Property<int>("HackathonEventId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ParticipantType")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<int>("SatisfactionIndex")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("HackathonEventId");

                    b.ToTable("Participants");

                    b.HasDiscriminator<string>("ParticipantType").HasValue("Participant");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Hackathon.Model.Preference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ParticipantId")
                        .HasColumnType("integer");

                    b.Property<string>("PreferredName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Rank")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("Wishlists");
                });

            modelBuilder.Entity("Hackathon.Model.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("HackathonEventId")
                        .HasColumnType("integer");

                    b.Property<int>("JuniorId")
                        .HasColumnType("integer");

                    b.Property<int>("TeamLeadId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("HackathonEventId");

                    b.HasIndex("JuniorId");

                    b.HasIndex("TeamLeadId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Hackathon.Model.Junior", b =>
                {
                    b.HasBaseType("Hackathon.Model.Participant");

                    b.HasDiscriminator().HasValue("Junior");
                });

            modelBuilder.Entity("Hackathon.Model.TeamLead", b =>
                {
                    b.HasBaseType("Hackathon.Model.Participant");

                    b.HasDiscriminator().HasValue("TeamLead");
                });

            modelBuilder.Entity("Hackathon.Model.Participant", b =>
                {
                    b.HasOne("Hackathon.Model.HackathonEvent", "HackathonEvent")
                        .WithMany("Participants")
                        .HasForeignKey("HackathonEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HackathonEvent");
                });

            modelBuilder.Entity("Hackathon.Model.Preference", b =>
                {
                    b.HasOne("Hackathon.Model.Participant", "Participant")
                        .WithMany("Preferences")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("Hackathon.Model.Team", b =>
                {
                    b.HasOne("Hackathon.Model.HackathonEvent", "HackathonEvent")
                        .WithMany("Teams")
                        .HasForeignKey("HackathonEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hackathon.Model.Junior", "Junior")
                        .WithMany()
                        .HasForeignKey("JuniorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Hackathon.Model.TeamLead", "TeamLead")
                        .WithMany()
                        .HasForeignKey("TeamLeadId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("HackathonEvent");

                    b.Navigation("Junior");

                    b.Navigation("TeamLead");
                });

            modelBuilder.Entity("Hackathon.Model.HackathonEvent", b =>
                {
                    b.Navigation("Participants");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("Hackathon.Model.Participant", b =>
                {
                    b.Navigation("Preferences");
                });
#pragma warning restore 612, 618
        }
    }
}
