
namespace beginner.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character> {
            new Character(),
            new Character { Id = 3, Name="Sam"}
        };

        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var character = _mapper.Map<Character>(newCharacter);
            var dbCharacters = await _context.Characters.ToListAsync();
            if (dbCharacters.Count > 0) character.Id = dbCharacters.Max(c => c.Id) + 1;
            else character.Id = 0;
            dbCharacters.Add(character);
            dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {

            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            try
            {

                var dbCharacters = await _context.Characters.ToListAsync();
                var character = dbCharacters.First(c => c.Id == id);

                if (character == null)
                {
                    throw new Exception($"Character with id '{id}' not found.");
                }

                characters.Remove(character);

                serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;

        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var dbCharacters = await _context.Characters.ToListAsync();
                var character = dbCharacters.FirstOrDefault(c => c.Id == updateCharacter.Id);

                if (character == null)
                {
                    throw new Exception($"Character with id '{updateCharacter.Id}' not found.");
                }

                _mapper.Map<Character>(updateCharacter);
                _mapper.Map(updateCharacter, character);

                character.Name = updateCharacter.Name;
                character.HitPoints = updateCharacter.HitPoints;
                character.Strength = updateCharacter.Strength;
                character.Defense = updateCharacter.Defense;
                character.Intelligence = updateCharacter.Intelligence;
                character.Class = updateCharacter.Class;

                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;

        }
    }
}