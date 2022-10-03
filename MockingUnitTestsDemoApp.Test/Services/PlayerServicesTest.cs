using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using MockingUnitTestsDemoApp.Impl.Services.Interfaces;
using NSubstitute;

namespace MockingUnitTestsDemoApp.Test.Services
{
    public class PlayerServicesTest
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ILeagueRepository _leagueRepository;

        private readonly IPlayerService _subject;

        private static int teamIdValido_1 = 1;
        private static int teamIdValido_2 = 2;
        private static int teamIdInvalido = 3;
        private static int teamIdValido_4 = 4;
        private static int teamIdValido_5 = 5;

        private static List<Player> listPlayersIdValido_1 = new List<Player>()
        {
            new Player() {
                ID = 1,
                FirstName = "Ana",
                LastName = "Mello",
                DateOfBirth = new DateTime(1992, 10, 10),
                TeamID = teamIdValido_1
            },
            new Player(){
                ID = 2,
                FirstName = "xxxx",
                LastName = "yyyy",
                DateOfBirth = new DateTime(1992, 11, 11),
                TeamID = teamIdValido_1
            }
        };

        private static List<Player> listPlayersIdValido_2 = new List<Player>()
        {
            new Player()
            {
                ID = 3,
                FirstName = "uuuu",
                LastName = "llll",
                DateOfBirth = new DateTime(1992, 12, 12),
                TeamID = teamIdValido_2
            }
        };

        private static List<Player> listPlayersIdInvalido = null;


        private IPlayerRepository CreatePlayerRepositoryMock()
        {
            var playerRepository = Substitute.For<IPlayerRepository>();
            playerRepository.GetForTeam(teamIdValido_1).Returns(listPlayersIdValido_1);
            playerRepository.GetForTeam(teamIdValido_2).Returns(listPlayersIdValido_2);
            playerRepository.GetForTeam(teamIdInvalido).Returns(listPlayersIdInvalido);
            playerRepository.GetForTeam(teamIdValido_4).Returns(listPlayersIdInvalido);
            playerRepository.GetForTeam(teamIdValido_5).Returns(listPlayersIdInvalido);

            return playerRepository;
        }
        //--------------------------------

        private static int leagueIdValido = 1;
        private static int leagueIdInvalido = 2;
        private static int leagueIdValidoSemTime = 3;
        private static int leagueIdValidoComTimeSemPlayers = 4;

        private static List<Team> listTeamIdValido = new List<Team>()
        {
            new Team()
            {
                ID = 1,
                Name = "wwwww",
                LeagueID = leagueIdValido,
                FoundingDate = new DateTime(2020,10,10)
            },
            new Team()
            {
                ID = 2,
                Name = "gggggggg",
                LeagueID = leagueIdValido,
                FoundingDate = new DateTime(2020,10,10)
            }
        };

        private static List<Team> listTeamIdInvalido = null;

        private ITeamRepository CreateTeamReposotoryMock()
        {
            var TeamRepository = Substitute.For<ITeamRepository>();
            TeamRepository.GetForLeague(leagueIdValido).Returns(listTeamIdValido);
            TeamRepository.GetForLeague(leagueIdValidoSemTime).Returns(new List<Team>());
            TeamRepository.GetForLeague(leagueIdInvalido).Returns(listTeamIdInvalido);

            TeamRepository.GetForLeague(leagueIdValidoComTimeSemPlayers).Returns(listTeamsSemPlayers);

            return TeamRepository;

        }

        //----------------------------------------

        private static League validLeage = new()
        {
            ID = leagueIdValido,
            Name = "kkllll",
            FoundingDate = new DateTime(2015, 1, 2)
        };

        private static League validLeageSemTime = new()
        {
            ID = leagueIdValidoSemTime,
            Name = "kkllll",
            FoundingDate = new DateTime(2015, 1, 2)
        };
        private List<Team> listTeamsSemPlayers = new List<Team>()
        {
            new Team()
            {
                ID=teamIdValido_4
            },
            new Team()
            {
                ID=teamIdValido_5
            }
        };

        private ILeagueRepository CreateLeagueReposotoryMock()
        {
            League nullLeague = null;

            var leagueRepository = Substitute.For<ILeagueRepository>();
            leagueRepository.GetByID(leagueIdValido).Returns(validLeage);
            leagueRepository.GetByID(leagueIdInvalido).Returns(nullLeague);
            leagueRepository.GetByID(leagueIdValidoSemTime).Returns(validLeageSemTime);
            leagueRepository.IsValid(leagueIdValido).Returns(true);
            leagueRepository.IsValid(leagueIdInvalido).Returns(false);

            return leagueRepository;
        }

        //---------------------------------

        public PlayerServicesTest()
        {
            _playerRepository = CreatePlayerRepositoryMock();
            _teamRepository = CreateTeamReposotoryMock();
            _leagueRepository = CreateLeagueReposotoryMock();

            _subject = new PlayerService(_playerRepository, _teamRepository, _leagueRepository);
        }


        [Fact]
        //liga com time e com jogadores
        public void HappyDay()
        {
            //Act
            var result = _subject.GetForLeague(leagueIdValido);

            //Assert

            Assert.NotNull(result);
            Assert.True(result.Intersect(listPlayersIdValido_1).Count() == listPlayersIdValido_1.Count());
            Assert.True(result.Intersect(listPlayersIdValido_2).Count() == listPlayersIdValido_2.Count());
        }

        [Fact]
        //liga invalida
        public void GetForLeague_LeagueIsInvalid_EmptyList()
        {
            //Act
            var result = _subject.GetForLeague(leagueIdInvalido);
            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        //liga sem time
        public void GetForLeague_LeagueWithoutTeam_leagueListWithoutTeams()
        {
            //Act
            var result = _subject.GetForLeague(leagueIdValidoSemTime);
            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);

        }

        [Fact]
        //liga com time, mas sem jogadores
        public void GetForLeague_LeagueWithoutPLayers_ListOfTeamsWithoutPlayers()
        {
            //Act
            var result = _subject.GetForLeague(leagueIdValidoComTimeSemPlayers);
            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);

        }
    }
}
