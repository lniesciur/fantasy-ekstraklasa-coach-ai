#!/usr/bin/env python3
"""
Script to find player name mismatches between players table and player_stats inserts
"""

import re

def extract_player_names_from_seed():
    """Extract player names from the seed.sql file"""
    
    # Read the seed file
    with open('supabase/seed.sql', 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Extract player names from INSERT INTO players
    players_pattern = r"INSERT INTO players \(name, team_id, position, price, health_status\) VALUES\s*\n(.*?);"
    players_match = re.search(players_pattern, content, re.DOTALL)
    
    players_names = set()
    if players_match:
        players_section = players_match.group(1)
        # Extract names from the VALUES section
        name_pattern = r"\('([^']+)',"
        for match in re.finditer(name_pattern, players_section):
            players_names.add(match.group(1))
    
    # Extract player names from player_stats SELECT queries
    stats_pattern = r"SELECT id FROM players WHERE name LIKE '%([^%]+)%'"
    stats_names = set()
    for match in re.finditer(stats_pattern, content):
        stats_names.add(match.group(1))
    
    # Find mismatches
    missing_in_players = stats_names - players_names
    missing_in_stats = players_names - stats_names
    
    print("Players in player_stats but not in players table:")
    for name in sorted(missing_in_players):
        print(f"  - {name}")
    
    print(f"\nTotal missing in players: {len(missing_in_players)}")
    print(f"Total players in players table: {len(players_names)}")
    print(f"Total players referenced in stats: {len(stats_names)}")
    
    return missing_in_players, players_names, stats_names

if __name__ == "__main__":
    missing_in_players, players_names, stats_names = extract_player_names_from_seed()
