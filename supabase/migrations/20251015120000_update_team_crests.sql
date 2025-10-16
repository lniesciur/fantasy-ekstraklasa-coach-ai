-- ===================================
-- supabase/migrations/20251015120000_update_team_crests.sql
-- ===================================

-- Aktualizacja herbów drużyn Ekstraklasy
-- Format URL: https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/[kod_drużyny]_[nazwa].png

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
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/06_Cracovia.png'
WHERE name = 'Cracovia';

-- Korona Kielce
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/07_Korona_Kielce.png'
WHERE name = 'Korona Kielce';

-- Wisła Płock
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/08_Wisla_Plock.png'
WHERE name = 'Wisła Płock';

-- Lech Poznań
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/09_Lech_Poznan.png'
WHERE name = 'Lech Poznań';

-- Legia Warszawa
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/10_Legia_Warszawa.png'
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
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/13_Zaglebie_Lubin.png'
WHERE name = 'Zagłębie Lubin';

-- Widzew Łódź
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/14_Widzew_Lodz.png'
WHERE name = 'Widzew Łódź';

-- Pogoń Szczecin
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/15_Pogon_Szczecin.png'
WHERE name = 'Pogoń Szczecin';

-- Arka Gdynia
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/16_Arka_Gdynia.png'
WHERE name = 'Arka Gdynia';

-- Motor Lublin
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/17_Motor_Lublin.png'
WHERE name = 'Motor Lublin';

-- Termalica B-B
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/18_Termalica_BB.png'
WHERE name = 'Termalica B-B';

-- GKS Katowice
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/19_GKS_Katowice.png'
WHERE name = 'GKS Katowice';

-- Piast Gliwice
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/20_Piast_Gliwice.png'
WHERE name = 'Piast Gliwice';

-- Lechia Gdańsk
UPDATE teams 
SET crest_url = 'https://d2vzq0pwftw3zc.cloudfront.net/fit-in/100x100/filters:quality(30)/de5a136b-59d1-40ce-8b51-c043a004751b/2021_2022/clubs/21_Lechia_Gdansk.png'
WHERE name = 'Lechia Gdańsk';

-- Komentarz dla dokumentacji
COMMENT ON COLUMN teams.crest_url IS 'URL do herbu drużyny - obrazek 100x100px z CloudFront CDN';
