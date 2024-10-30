// File: Tests/DataLoaderTests/DataLoaderTests.cs

using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Hackathon.Model;
using Hackathon.Options;
using Hackathon.Services;
using Xunit;

namespace Hackathon.Tests.DataLoaderTests;

public class DataLoaderTests
{
    [Fact]
    public void LoadJuniors_ShouldReturnCorrectList()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\data\juniors.txt", new MockFileData("Id;Name\n1;Junior1\n2;Junior2\n3;Junior3") }
        });
        
        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = @"C:\data\juniors.txt",
            TeamLeadsFilePath = "invalid_path"
        });

        var dataLoader = new DataLoader(options, mockFileSystem);

        // Act
        var juniors = dataLoader.LoadJuniors();

        // Assert
        Assert.Equal(3, juniors.Count);
        Assert.Contains(juniors, j => j.Name == "Junior1");
        Assert.Contains(juniors, j => j.Name == "Junior2");
        Assert.Contains(juniors, j => j.Name == "Junior3");
    }

    [Fact]
    public void LoadTeamLeads_ShouldReturnCorrectList()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\data\teamleads.txt", new MockFileData("Id;Name\n1;TeamLead1\n2;TeamLead2") }
            
        });

        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = "invalid_path",
            TeamLeadsFilePath = @"C:\data\teamleads.txt"
        });

        var dataLoader = new DataLoader(options, mockFileSystem);

        // Act
        var teamLeads = dataLoader.LoadTeamLeads();

        // Assert
        Assert.Equal(2, teamLeads.Count);
        Assert.Contains(teamLeads, tl => tl.Name == "TeamLead1");
        Assert.Contains(teamLeads, tl => tl.Name == "TeamLead2");
    }

    [Fact]
    public void LoadJuniors_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(); // Нет файлов

        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = @"C:\data\juniors.txt",
            TeamLeadsFilePath = @"C:\data\teamleads.txt"
        });
        
        var dataLoader = new DataLoader(options, mockFileSystem);
        
        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => dataLoader.LoadJuniors());
        Assert.Contains(@"C:\data\juniors.txt", exception.Message);
    }

    [Fact]
    public void LoadTeamLeads_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(); // Нет файлов

        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = @"C:\data\juniors.txt",
            TeamLeadsFilePath = @"C:\data\teamleads.txt"
        });

        var dataLoader = new DataLoader(options, mockFileSystem);

        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => dataLoader.LoadTeamLeads());
        Assert.Contains(@"C:\data\teamleads.txt", exception.Message);
    }

    [Fact]
    public void LoadJuniors_ShouldIgnoreEmptyLines()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\data\juniors.txt", new MockFileData("Id;Name\n1;Junior1\n\n2;Junior2\n   \n3;Junior3") }
        });
        
        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = @"C:\data\juniors.txt",
            TeamLeadsFilePath = "invalid_path"
        });


        var dataLoader = new DataLoader(options, mockFileSystem);
        
        // Act
        var juniors = dataLoader.LoadJuniors();

        // Assert
        Assert.Equal(3, juniors.Count);
        Assert.Contains(juniors, j => j.Name == "Junior1");
        Assert.Contains(juniors, j => j.Name == "Junior2");
        Assert.Contains(juniors, j => j.Name == "Junior3");
    }

    [Fact]
    public void LoadTeamLeads_ShouldIgnoreEmptyLines()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\data\teamleads.txt", new MockFileData("Id;Name\n1;TeamLead1\n\n2;TeamLead2\n   \n") }
        });

        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = "invalid_path",
            TeamLeadsFilePath = @"C:\data\teamleads.txt"
        });
        
        var dataLoader = new DataLoader(options, mockFileSystem);

        // Act
        var teamLeads = dataLoader.LoadTeamLeads();

        // Assert
        Assert.Equal(2, teamLeads.Count);
        Assert.Contains(teamLeads, tl => tl.Name == "TeamLead1");
        Assert.Contains(teamLeads, tl => tl.Name == "TeamLead2");
    }
    
    [Fact]
    public void LoadJuniors_ShouldHandleIncorrectFormatGracefully()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\data\juniors.txt", new MockFileData("Id;Name\n1;Junior1\nInvalidLine\n3;Junior3") }
        });

        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = @"C:\data\juniors.txt",
            TeamLeadsFilePath = @"C:\data\teamleads.txt"
        });

        var dataLoader = new DataLoader(options, mockFileSystem);

        // Act
        var juniors = dataLoader.LoadJuniors();

        // Assert
        Assert.Equal(2, juniors.Count);
        Assert.Contains(juniors, j => j.Name == "Junior1");
        Assert.Contains(juniors, j => j.Name == "Junior3");
    }

    [Fact]
    public void LoadTeamLeads_ShouldHandleIncorrectFormatGracefully()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\data\teamleads.txt", new MockFileData("Id;Name\n1;TeamLead1\nInvalidLine\n3;TeamLead3") }
        });

        var options = Microsoft.Extensions.Options.Options.Create(new DataLoaderOptions
        {
            JuniorsFilePath = @"C:\data\juniors.txt",
            TeamLeadsFilePath = @"C:\data\teamleads.txt"
        });

        var dataLoader = new DataLoader(options, mockFileSystem);

        // Act
        var teamLeads = dataLoader.LoadTeamLeads();

        // Assert
        Assert.Equal(2, teamLeads.Count);
        Assert.Contains(teamLeads, tl => tl.Name == "TeamLead1");
        Assert.Contains(teamLeads, tl => tl.Name == "TeamLead3");
    }
}
