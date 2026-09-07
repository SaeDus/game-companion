use serde_json::{Map, Value};
use tauri::Manager;
use std::fs;
use serde::Deserialize;

#[derive(Debug, Deserialize)]
struct CharacterManifest {
    #[serde(rename  = "GameId")]
    game_id: String,

    #[serde(rename = "CompanionId")]
    companion_id: String,

    #[serde(rename = "Includes")]
    includes: Vec<String>,
}

#[derive(Debug, Deserialize)]
struct ContentManifest {
    #[serde(rename = "Includes")]
    includes: Vec<String>,
}

fn find_json_by_id(
    directory: &std::path::Path,
    target_id: &str,
) -> Result<Value, String> {
    let entries = fs::read_dir(directory)
        .map_err(|error| error.to_string())?;

    for entry in entries {
        let entry = entry.map_err(|error| error.to_string())?;
        let path = entry.path();

        if path.is_dir() {
            if let Ok(value) = find_json_by_id(&path, target_id) {
                return Ok(value);
            }

            continue;
        }

        if path.extension().and_then(|ext| ext.to_str()) != Some("json") {
            continue;
        }

        let text = match fs::read_to_string(&path) {
            Ok(text) => text,
            Err(_) => continue,
        };

        let value: Value = match serde_json::from_str(&text) {
            Ok(value) => value,
            Err(_) => continue,
        };

        let id = value
            .get("Id")
            .and_then(Value::as_str);

        if id == Some(target_id) {
            return Ok(value);
        }
    }

    Err(format!(
        "Could not find JSON with Id: '{}'",
        target_id
    ))
}

fn strip_metadata(mut value: Value) -> Result<(String, Value), String> {
    let object = value
        .as_object_mut()
        .ok_or("Expected JSON object")?;

    let id = object
        .shift_remove("Id")
        .and_then(|value| value.as_str().map(String::from))
        .ok_or("JSON file is missing Id")?;

    object.shift_remove("SchemaVersion");

    if let Some(sections) = object
        .get_mut("Sections")
        .and_then(Value::as_array_mut)
    {
        for section in sections {
            let Some(section_object) = section.as_object_mut() else {
                continue;
            };

            section_object.shift_remove("Id");

            if let Some(rules) = section_object
                .get_mut("Rules")
                .and_then(Value::as_array_mut)
            {
                for rule in rules {
                    if let Some(rule_object) = rule.as_object_mut() {
                        rule_object.shift_remove("Id");
                    }
                }
            }
        }
    }

    Ok((id, value))
}

// Learn more about Tauri commands at https://tauri.app/develop/calling-rust/
#[tauri::command]
fn greet(name: &str) -> String {
    format!("Hello, {}! You've been greeted from Rust!", name)
}

#[tauri::command]
fn build_game_companion(
    app: tauri::AppHandle,
    character_id: String,
    character_json: String,
) -> Result<String, String> {
    let app_data = app
        .path()
        .local_data_dir()
        .map_err(|error| error.to_string())?
        .join("game-companion");

    // retrieves (manually) character.json from app data directory
    let character_dir = app_data
        .join("characters")
        .join(&character_id);

    let character_text = fs::read_to_string(&character_dir.join("character.json"))
        .map_err(|error| error.to_string())?;

    let manifest: CharacterManifest =
        serde_json::from_str(&character_text)
            .map_err(|error| error.to_string())?;
    
    // retrieves game.json assigned to character.json
    let game_dir = app_data
        .join("games")
        .join(&manifest.game_id);

    let game_text = fs::read_to_string(&game_dir.join("game.json"))
        .map_err(|error| error.to_string())?;

    let game_manifest: ContentManifest =
        serde_json::from_str(&game_text)
            .map_err(|error| error.to_string())?;
    
    // retrieves companion.json assigned to character.json
    let companion_dir = app_data
        .join("companions")
        .join(&manifest.companion_id);

    let companion_text = fs::read_to_string(&companion_dir.join("companion.json"))
        .map_err(|error| error.to_string())?;

    let companion_manifest: ContentManifest =
        serde_json::from_str(&companion_text)
            .map_err(|error| error.to_string())?;
    
    // gather all rules together
    let mut character_content = Map::new();

    for include_id in &manifest.includes {
        let file = find_json_by_id(
            &character_dir,
            include_id,
        )?;

        let (id, content) = strip_metadata(file)?;

        character_content.insert(id, content);
    }
    
    let mut game_content = Map::new();

    for include_id in &game_manifest.includes {
        let file = find_json_by_id(
            &game_dir,
            include_id,
        )?;

        let (id, content) = strip_metadata(file)?;

        game_content.insert(id, content);
    }

    let mut companion_content = Map::new();

    for include_id in &companion_manifest.includes {
        let file = find_json_by_id(
            &companion_dir,
            include_id,
        )?;

        let (id, content) = strip_metadata(file)?;

        companion_content.insert(id, content);
    }

    let authority_file = find_json_by_id(&app_data.join("authority"), "authority")?;
    let (id, content) = strip_metadata(authority_file)?;

    let mut rules_content = Map::new();
    rules_content.insert(id, content);
    rules_content.extend(companion_content);
    rules_content.extend(game_content);
    rules_content.extend(character_content);

    let rules_json = Value::Object(rules_content);

    // combine character with rules for final json output
    let character_data: Value =
        serde_json::from_str(&character_json)
            .map_err(|error| error.to_string())?;

    let mut final_content = Map::new();

    final_content.insert(
        "character".to_string(),
        character_data,
    );

    final_content.insert(
        "rules".to_string(),
        rules_json,
    );

    let final_json = Value::Object(final_content);

    Ok(
        serde_json::to_string_pretty(&final_json)
            .map_err(|error| error.to_string())?
    )
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_store::Builder::new().build())
        .plugin(tauri_plugin_shell::init())
        .plugin(tauri_plugin_fs::init())
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_opener::init())
        .invoke_handler(tauri::generate_handler![
            greet,
            build_game_companion
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
