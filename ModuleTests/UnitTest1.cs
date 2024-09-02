using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Controllers;
using WebApplication1.Entities;
using WebApplication1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Xml.Linq;
using System.Text.Json;
using WebApplication1.JsonConverter;
using System.Diagnostics;

namespace WebApplication1.Tests.Controllers
{
    public class NoteControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public NoteControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetNotes_WhenNoNotes_ReturnsEmptyList()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("UserId", "61");
            CurrentUser.UserId = 61;

            // Act
            var response = await client.GetAsync("/Note/GetNotes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };
            var notes = await response.Content.ReadFromJsonAsync<List<Note>>(options);
            notes.Should().BeEmpty();
            Console.WriteLine($"Статус код: {response.StatusCode}");
            Console.WriteLine($"Полученные данные: {response.Content.ReadAsStringAsync().Result}"); // Вывод содержимого ответа
        }

        [Fact]
        public async Task GetNotes_WhenNotesExist_ReturnsListOfNotes()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("UserId", "55");
            CurrentUser.UserId = 55;

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NoteRepository>();
            var context1 = scope.ServiceProvider.GetRequiredService<ElementRepository>();
    
            // Act
            var response = await client.GetAsync("/Note/GetNotes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };

            var notes = await response.Content.ReadFromJsonAsync<List<Note>>(options);
            notes.Count.Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetNote_WhenNoteExists_ReturnsNote()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("UserId", "55");
            CurrentUser.UserId = 55;

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NoteRepository>();
            var context1 = scope.ServiceProvider.GetRequiredService<ElementRepository>();
            Element newElem1 = new Element();
            newElem1.user_id = 55;
            newElem1.elem_type = false;
            context1.Elements.Add(newElem1);
            context1.SaveChanges();

            int newElemId = newElem1.elem_id;
            var note = new Note { elem_id = newElemId, note_name = "Test Note 1", note_create = DateOnly.FromDateTime(DateTime.Now), description = "This is a test note 1." };
            context.Notes.Add(note);
            context.SaveChanges();

            // Act
            var response = await client.GetAsync("/Note/GetNote/177");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };
            var returnedNote = await response.Content.ReadFromJsonAsync<Note>(options);
            returnedNote.note_name.Should().Be("Test Note 1");
            returnedNote.description.Should().Be("This is a test note 1.");
        }

        [Fact]
        public async Task GetNote_WhenNoteDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("UserId", "55");
            CurrentUser.UserId = 55;

            // Act
            var response = await client.GetAsync("/Note/GetNote/1000");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateNote_WhenValidModel_CreatesNote()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("UserId", "55");
            CurrentUser.UserId = 55;
            var newNote = new NoteCreateModel { note_name = "Test Note 2", description = "This is a test note 2" };

            // Act
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyConverter() }
            };
            var response = await client.PostAsJsonAsync("/Note/CreateNote", newNote);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var createdNote = await response.Content.ReadFromJsonAsync<Note>(options);
            createdNote.note_name.Should().Be("Test Note 2");
            createdNote.description.Should().Be("This is a test note 2");

        }

        [Fact]
        public async Task CreateNote_WhenInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("UserId", "55");
            CurrentUser.UserId = 55;
            var newNote = new NoteCreateModel();

            // Act
            var response = await client.PostAsJsonAsync("/Note/CreateNote", newNote);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}