﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------


namespace TextBook.Data
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class DBTextBookEntities : DbContext
{
    public DBTextBookEntities()
        : base("name=DBTextBookEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<Autorization> Autorization { get; set; }

    public virtual DbSet<Test> Test { get; set; }

    public virtual DbSet<TestAnswer> TestAnswer { get; set; }

    public virtual DbSet<TestQuestion> TestQuestion { get; set; }

    public virtual DbSet<TestResult> TestResult { get; set; }

    public virtual DbSet<Theme> Theme { get; set; }

    public virtual DbSet<TopicTest> TopicTest { get; set; }

    public virtual DbSet<ImageTheme> ImageTheme { get; set; }

}

}

