-- ===================================
-- supabase/migrations/20251015130000_update_team_crests_new.sql
-- ===================================

-- Aktualizacja herbów drużyn Ekstraklasy - nowe URL
-- Tutaj możesz dodać nowe URL-e herbów drużyn

-- Górnik Zabrze
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/04_Gornik_Zabrze.png'
WHERE name = 'Górnik Zabrze';

-- Jagiellonia Białystok
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/05_Jagiellonia_Bialystok.png'
WHERE name = 'Jagiellonia Białystok';

-- Cracovia
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1751376817_03_Cracovia_update.png'
WHERE name = 'Cracovia';

-- Korona Kielce
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1719319482_05_Korona_Kielce.png'
WHERE name = 'Korona Kielce';

-- Wisła Płock
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/17_Wisla_Plock.png'
WHERE name = 'Wisła Płock';

-- Lech Poznań
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2022_23/clubs/05_Lech_Pozna%C5%84.png'
WHERE name = 'Lech Poznań';

-- Legia Warszawa
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/Herb_Legia_Warszawa_2.png'
WHERE name = 'Legia Warszawa';

-- Radomiak Radom
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/11_Radomiak_Radom.png'
WHERE name = 'Radomiak Radom';

-- Raków Częstochowa
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/12_Rakow_Czestochowa.png'
WHERE name = 'Raków Częstochowa';

-- Zagłębie Lubin
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1750169173_18_Zaglebie_Lubin_474px.png'
WHERE name = 'Zagłębie Lubin';

-- Widzew Łódź
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2022_23/clubs/Widzew-Lodz-herb.png'
WHERE name = 'Widzew Łódź';

-- Pogoń Szczecin
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1719319351_11_Pogon_Szczecin.png'
WHERE name = 'Pogoń Szczecin';

-- Arka Gdynia
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1749722866_01_Arka_Gdynia.png'
WHERE name = 'Arka Gdynia';

-- Motor Lublin
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1751291906_motor_lublin_biale.png'
WHERE name = 'Motor Lublin';

-- Termalica B-B
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/01_Bruk-bet_Termalica_Nieciecza.png'
WHERE name = 'Termalica B-B';

-- GKS Katowice
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1735818936_02_GKS_Katowice.png'
WHERE name = 'GKS Katowice';

-- Piast Gliwice
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1749722866_12_Piast_Gliwice.png'
WHERE name = 'Piast Gliwice';

-- Lechia Gdańsk
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/1754555913_lechia_herb_jubileuszowy.png'
WHERE name = 'Lechia Gdańsk';

-- Komentarz dla dokumentacji
COMMENT ON COLUMN teams.crest_url IS 'URL do herbu drużyny - obrazek 100x100px z CloudFront CDN';
