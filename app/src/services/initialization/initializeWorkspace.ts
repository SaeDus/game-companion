import {
  BaseDirectory,
  copyFile,
  exists,
  mkdir,
  readDir,
} from "@tauri-apps/plugin-fs";

const RESOURCE_DEFAULTS_DIRECTORY = "defaults";
const WORKSPACE_DIRECTORY = "game-companion";
const CHARACTERS_DIRECTORY = "characters";
const SKIPPED_ROOT_DIRECTORIES = new Set(["templates"]);

export interface InitializationResult {
  createdDirectories: string[];
  copiedFiles: string[];
  skippedFiles: string[];
}

function joinRelativePath(...parts: string[]): string {
  return parts.filter(Boolean).join("/");
}

async function ensureWorkspaceDirectory(
  relativePath: string,
  result: InitializationResult
): Promise<void> {
  const destinationPath = joinRelativePath(
    WORKSPACE_DIRECTORY,
    relativePath
  );

  const directoryExists = await exists(destinationPath, {
    baseDir: BaseDirectory.LocalData,
  });

  if (directoryExists) {
    return;
  }

  await mkdir(destinationPath, {
    baseDir: BaseDirectory.LocalData,
    recursive: true,
  });

  result.createdDirectories.push(destinationPath);
}

async function copyMissingDefaults(
  sourcePath: string,
  destinationRelativePath: string,
  result: InitializationResult,
  isRoot = false
): Promise<void> {
  await ensureWorkspaceDirectory(destinationRelativePath, result);

  const entries = await readDir(sourcePath, {
    baseDir: BaseDirectory.Resource,
  });

  for (const entry of entries) {
    if (
      isRoot &&
      entry.isDirectory &&
      SKIPPED_ROOT_DIRECTORIES.has(entry.name)
    ) {
      continue;
    }

    const sourceEntryPath = joinRelativePath(sourcePath, entry.name);
    const destinationEntryRelativePath = joinRelativePath(
      destinationRelativePath,
      entry.name
    );
    const destinationEntryPath = joinRelativePath(
      WORKSPACE_DIRECTORY,
      destinationEntryRelativePath
    );

    if (entry.isDirectory) {
      await copyMissingDefaults(
        sourceEntryPath,
        destinationEntryRelativePath,
        result
      );
      continue;
    }

    if (!entry.isFile) {
      continue;
    }

    const destinationExists = await exists(destinationEntryPath, {
      baseDir: BaseDirectory.LocalData,
    });

    if (destinationExists) {
      result.skippedFiles.push(destinationEntryPath);
      continue;
    }

    await copyFile(sourceEntryPath, destinationEntryPath, {
      fromPathBaseDir: BaseDirectory.Resource,
      toPathBaseDir: BaseDirectory.LocalData,
    });

    result.copiedFiles.push(destinationEntryPath);
  }
}

export async function initializeWorkspace(): Promise<InitializationResult> {
  const result: InitializationResult = {
    createdDirectories: [],
    copiedFiles: [],
    skippedFiles: [],
  };

  await copyMissingDefaults(
    RESOURCE_DEFAULTS_DIRECTORY,
    "",
    result,
    true
  );

  await ensureWorkspaceDirectory(CHARACTERS_DIRECTORY, result);

  return result;
}
