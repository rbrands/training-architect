/**
 * ComponentMapper: BM25-based component search for Stage 4
 *
 * Maps user queries (element type + content hints) to Syncfusion Blazor components
 * using BM25 ranking on component keywords.
 *
 * Usage:
 *   // Direct import for Stage 4
 *   const { ComponentMapper, stage4ComponentPicking } = require('./components_search.cjs');
 *
 *   const componentMappingJson = {...}; // From Stage 3 (component-mapping.json)
 *   const result = stage4ComponentPicking(componentMappingJson);
 *   // Returns: Stage 4 output JSON with mapped components
 *
 *   // Or standalone CLI
 *   const mapper = new ComponentMapper('components.csv');
 *   const results = mapper.search('button primary action', 5);
 *   // Returns: [[componentName, skillName, score], ...]
 */

'use strict';

const fs   = require('fs');
const path = require('path');


// ---------------------------------------------------------------------------
// BM25
// ---------------------------------------------------------------------------

/**
 * BM25 ranking algorithm for component keyword search.
 */
class BM25 {
  /**
   * @param {string[]} documents - List of documents (keyword strings)
   * @param {number}   k1        - Saturation parameter (default 1.5)
   * @param {number}   b         - Length normalization parameter (default 0.75)
   */
  constructor(documents, k1 = 1.5, b = 0.75) {
    this.k1 = k1;
    this.b  = b;
    this.documents   = documents;
    this.corpusSize  = documents.length;

    this.tokenizedDocs = documents.map(doc => this._tokenize(doc));
    this.idfCache      = {};
    this.avgDocLength  = this.tokenizedDocs.reduce((sum, d) => sum + d.length, 0) /
                         Math.max(1, this.corpusSize);

    this._computeIdf();
  }

  /** Simple tokenization: split on whitespace and commas. */
  _tokenize(text) {
    return text.toLowerCase().replace(/,/g, ' ').split(/\s+/).filter(t => t.length > 0);
  }

  /** Precompute IDF scores for all terms. */
  _computeIdf() {
    const termDocCount = {};

    for (const doc of this.tokenizedDocs) {
      for (const term of new Set(doc)) {
        termDocCount[term] = (termDocCount[term] || 0) + 1;
      }
    }

    for (const [term, docCount] of Object.entries(termDocCount)) {
      // BM25 IDF formula: log((N - n(t) + 0.5) / (n(t) + 0.5) + 1.0)
      this.idfCache[term] = Math.log(
        (this.corpusSize - docCount + 0.5) / (docCount + 0.5) + 1.0
      );
    }
  }

  /**
   * Score a document against a query using BM25.
   * @param {string} query     - Query string
   * @param {number} docIndex  - Index of document to score
   * @returns {number} BM25 score (higher = better match)
   */
  score(query, docIndex) {
    const queryTerms = this._tokenize(query);
    const doc        = this.tokenizedDocs[docIndex];
    const docLength  = doc.length;

    let total = 0.0;
    for (const term of queryTerms) {
      const idf      = this.idfCache[term] || 0.0;
      const termFreq = doc.filter(t => t === term).length;

      const numerator   = idf * termFreq * (this.k1 + 1);
      const denominator = termFreq + this.k1 * (1 - this.b + this.b * (docLength / this.avgDocLength));

      total += numerator / denominator;
    }

    return total;
  }
}


// ---------------------------------------------------------------------------
// ComponentMapper
// ---------------------------------------------------------------------------

/**
 * Map user queries to Syncfusion Blazor components using BM25 search.
 */
class ComponentMapper {
  /**
   * @param {string} csvPath - Path to components.csv file
   */
  constructor(csvPath = 'components.csv') {
    this.csvPath = path.resolve(csvPath);

    if (!fs.existsSync(this.csvPath)) {
      throw new Error(`Components CSV not found: ${csvPath}`);
    }

    this.components   = [];
    this.keywordsList = [];

    const rows = parseCsv(this.csvPath);
    for (const row of rows) {
      this.components.push({
        id:       row['Component ID'],
        name:     row['Component Name'],
        skill:    row['Skill Name'],
        keywords: row['Keywords'],
      });
      this.keywordsList.push(row['Keywords']);
    }

    if (this.components.length === 0) {
      throw new Error('No components found in CSV');
    }

    this.bm25 = new BM25(this.keywordsList, 1.5, 0.75);
  }

  /**
   * Search for components matching the query.
   * @param {string} query  - User query (e.g. "button primary action")
   * @param {number} topK   - Number of top results to return (default 5)
   * @returns {Array<[string, string, number]>} [[componentName, skillName, score], ...]
   */
  search(query, topK = 5) {
    if (!query || !query.trim()) return [];

    const scores = [];
    for (let i = 0; i < this.components.length; i++) {
      const score = this.bm25.score(query, i);
      if (score > 0) {
        const comp = this.components[i];
        scores.push([comp.name, comp.skill, score]);
      }
    }

    scores.sort((a, b) => b[2] - a[2]);
    return scores.slice(0, topK);
  }

  /**
   * Get component skill by exact component name.
   * @param {string} componentName
   * @returns {[string, string] | null}
   */
  getByName(componentName) {
    for (const comp of this.components) {
      if (comp.name === componentName) return [comp.name, comp.skill];
    }
    return null;
  }

  /**
   * Get all components for a specific skill.
   * @param {string} skillName
   * @returns {Array<[string, string]>}
   */
  getBySkill(skillName) {
    return this.components
      .filter(c => c.skill === skillName)
      .map(c => [c.name, c.skill]);
  }

  /** Get all unique skill names in the database. */
  listSkills() {
    return [...new Set(this.components.map(c => c.skill))].sort();
  }

  /** Get all components as [name, skill] pairs. */
  listAll() {
    return this.components.map(c => [c.name, c.skill]);
  }
}


// ---------------------------------------------------------------------------
// IconMapper
// ---------------------------------------------------------------------------

/**
 * Map user queries to Syncfusion EJ2 icons using BM25 search.
 */
class IconMapper {
  /**
   * @param {string} csvPath - Path to icons.csv file
   */
  constructor(csvPath = 'icons.csv') {
    this.csvPath = path.resolve(csvPath);

    if (!fs.existsSync(this.csvPath)) {
      throw new Error(`Icons CSV not found: ${csvPath}`);
    }

    this.icons        = [];
    this.keywordsList = [];

    const rows = parseCsv(this.csvPath);
    for (const row of rows) {
      this.icons.push({
        id:        row['Icon ID'],
        name:      row['Icon Name'],
        iconClass: row['Icon Class'],
        keywords:  row['Keywords'],
      });
      this.keywordsList.push(row['Keywords']);
    }

    if (this.icons.length === 0) {
      throw new Error('No icons found in CSV');
    }

    this.bm25 = new BM25(this.keywordsList, 1.5, 0.75);
  }

  /**
   * Search for icons matching the query.
   * @param {string} query  - User query (e.g. "save persist store")
   * @param {number} topK   - Number of top results to return (default 1)
   * @returns {Array<[string, string, number]>} [[iconName, iconClass, score], ...]
   */
  search(query, topK = 1) {
    if (!query || !query.trim()) return [];

    const scores = [];
    for (let i = 0; i < this.icons.length; i++) {
      const score = this.bm25.score(query, i);
      if (score > 0) {
        const icon = this.icons[i];
        scores.push([icon.name, icon.iconClass, score]);
      }
    }

    scores.sort((a, b) => b[2] - a[2]);
    return scores.slice(0, topK);
  }

  /**
   * Get icon class by exact icon name.
   * @param {string} iconName
   * @returns {[string, string] | null}
   */
  getByName(iconName) {
    for (const icon of this.icons) {
      if (icon.name === iconName) return [icon.name, icon.iconClass];
    }
    return null;
  }

  /**
   * Get all icons for a specific category.
   * @param {string} category
   * @returns {Array<[string, string]>}
   */
  getByCategory(category) {
    return this.icons
      .filter(i => i.category === category)
      .map(i => [i.name, i.iconClass]);
  }

  /** Get all icons with names, classes, and keywords. */
  listAll() {
    return this.icons.map(i => [i.name, i.iconClass, i.keywords]);
  }
}


// ---------------------------------------------------------------------------
// stage4ComponentPicking  (equivalent to stage_4_component_picking)
// ---------------------------------------------------------------------------

/**
 * Map Stage 3 component-mapping.json elements to Syncfusion components and EJ2 icons.
 *
 * @param {Object} layoutJson      - Stage 3 output JSON with components/sections/icon_elements
 * @param {string} csvPath         - Path to components.csv
 * @param {string} iconsCsvPath    - Path to icons.csv
 * @returns {Object} Dict with mapped_components, mapped_icon_elements, mapped_sections
 */
function stage4ComponentPicking(layoutJson, csvPath = 'components.csv', iconsCsvPath = 'icons.csv') {
  const mapper     = new ComponentMapper(csvPath);
  const iconMapper = new IconMapper(iconsCsvPath);

  const mappedComponents   = [];
  const mappedIconElements = [];
  const mappedSections     = [];

  // AUTO-FLATTEN: Handle both nested sections and flat elements
  const elementsToMap = [];
  const hasSections   = Array.isArray(layoutJson.sections) && layoutJson.sections.length > 0;

  if (hasSections) {
    // COMPLEX MAPPING: Extract elements from sections
    for (const section of layoutJson.sections) {
      const sectionId       = section.section_id || '';
      const sectionElements = [];

      for (const element of (section.elements || [])) {
        // Wrap with section context — do NOT mutate the original element object
        // so _section_id never leaks into the written-back component-mapping.json
        elementsToMap.push({ element, sectionId });
        sectionElements.push(element.id || '');
      }

      if (sectionElements.length > 0) {
        mappedSections.push({
          section_id:   sectionId,
          section_name: section.section_name || '',
          elements:     sectionElements,
        });
      }
    }
  } else {
    // SIMPLE MAPPING: Use flat elements array
    for (const el of (layoutJson.elements || [])) {
      elementsToMap.push({ element: el, sectionId: null });
    }
  }

  // MAP ELEMENTS TO COMPONENTS AND ICONS
  for (const { element, sectionId } of elementsToMap) {
    const query   = `${element.type_hint || ''} ${element.description || ''}`.trim();
    const results = mapper.search(query, 1);

    const componentMap = {
      element_id:   element.id,
      element_name: element.name,
    };

    if (sectionId !== null) {
      componentMap.section_id = sectionId;
    }

    if (results.length > 0) {
      const [componentName, skillName, score] = results[0];
      componentMap.component = componentName;
      componentMap.skill     = skillName;
      componentMap.score     = Math.round(score * 100) / 100;
    } else {
      componentMap.component = 'NATIVE_HTML';
      componentMap.skill     = null;
      componentMap.score     = 0;
    }

    // MAP ICON if icon_hint provided
    if (element.icon_hint) {
      const iconResults = iconMapper.search(element.icon_hint.trim(), 1);
      if (iconResults.length > 0) {
        const [iconName, iconClass, iconScore] = iconResults[0];
        componentMap.icon = {
          name:     iconName,
          iconCss:  iconClass,
          score:    Math.round(iconScore * 100) / 100,
        };
      }
    }

    mappedComponents.push(componentMap);
  }

  // MAP ICON-ONLY ELEMENTS
  for (const iconElement of (layoutJson.icon_elements || [])) {
    const iconMap = {
      element_id:   iconElement.id   || '',
      element_name: iconElement.name || '',
      element_type: iconElement.type || 'icon',
    };

    const iconQuery   = iconElement.icon_hint || '';
    const iconResults = iconMapper.search(iconQuery.trim(), 1);

    if (iconResults.length > 0) {
      const [iconName, iconClass, iconScore] = iconResults[0];
      iconMap.icon = {
        name:    iconName,
        iconCss: iconClass,
        score:   Math.round(iconScore * 100) / 100,
      };
    } else {
      iconMap.icon = {
        name:    'Unknown',
        iconCss: 'e-icons e-help',
        score:   0,
      };
    }

    mappedIconElements.push(iconMap);
  }

  // BUILD OUTPUT
  const output = {
    component_type:    layoutJson.component_type || 'Unknown',
    variant:           layoutJson.variant        || 'Default',
    mapped_components: mappedComponents,
  };

  if (mappedIconElements.length > 0) {
    output.mapped_icon_elements = mappedIconElements;
  }

  if (hasSections && mappedSections.length > 0) {
    output.mapped_sections = mappedSections;
  }

  return output;
}



// ---------------------------------------------------------------------------
// Minimal CSV parser  (no dependencies — replaces Python's csv.DictReader)
// ---------------------------------------------------------------------------

/**
 * Parse a CSV file into an array of objects keyed by header row.
 * Handles quoted fields (including commas and newlines inside quotes).
 *
 * @param {string} filePath - Absolute path to the CSV file
 * @returns {Object[]}
 */
function parseCsv(filePath) {
  const raw    = fs.readFileSync(filePath, 'utf8');
  const lines  = splitCsvLines(raw);
  if (lines.length === 0) return [];

  const headers = parseCsvLine(lines[0]);
  const rows    = [];

  for (let i = 1; i < lines.length; i++) {
    const line = lines[i];              // FIX: no .trim() — preserves whitespace in fields
    if (!line.trim()) continue;         // skip fully blank lines only
    const values = parseCsvLine(line);
    const row    = {};
    headers.forEach((h, idx) => {
      row[h] = values[idx] !== undefined ? values[idx] : '';
    });
    rows.push(row);
  }

  return rows;
}

/**
 * Split raw CSV text into logical lines, respecting quoted fields
 * that may contain embedded newlines.
 * Does NOT decode content — passes raw text to parseCsvLine unchanged.
 *
 * @param {string} text
 * @returns {string[]}
 */
function splitCsvLines(text) {
  const lines = [];
  let   start   = 0;
  let   inQuote = false;
  let   i       = 0;

  while (i < text.length) {
    const ch = text[i];
    if (ch === '"') {
      if (!inQuote) {
        inQuote = true;
      } else if (text[i + 1] === '"') {
        i += 2;    // escaped-quote pair: skip both, stay inside quoted field
        continue;
      } else {
        inQuote = false;
      }
    } else if ((ch === '\n' || ch === '\r') && !inQuote) {
      lines.push(text.slice(start, i));
      if (ch === '\r' && text[i + 1] === '\n') i++; // CRLF
      start = i + 1;
    }
    i++;
  }

  if (start < text.length) lines.push(text.slice(start));
  return lines;
}

/**
 * Parse a single CSV line into an array of field values.
 * Handles quoted fields with embedded commas and escaped quotes ("").
 *
 * @param {string} line
 * @returns {string[]}
 */
function parseCsvLine(line) {
  const fields = [];
  let   i      = 0;

  while (i < line.length) {
    if (line[i] === '"') {
      // Quoted field
      i++;
      let field = '';
      while (i < line.length) {
        if (line[i] === '"' && line[i + 1] === '"') {
          field += '"';
          i += 2;
        } else if (line[i] === '"') {
          i++;
          break;
        } else {
          field += line[i++];
        }
      }
      fields.push(field);
      if (line[i] === ',') i++; // skip comma after closing quote
    } else {
      // Unquoted field
      const end = line.indexOf(',', i);
      if (end === -1) {
        fields.push(line.slice(i));
        break;
      } else {
        fields.push(line.slice(i, end));
        i = end + 1;
      }
    }
  }

  return fields;
}


// ---------------------------------------------------------------------------
// Exports
// ---------------------------------------------------------------------------

module.exports = {
  BM25,
  ComponentMapper,
  IconMapper,
  stage4ComponentPicking,
  parseCsv,
};


// ---------------------------------------------------------------------------
// writeBackToJson
// ---------------------------------------------------------------------------

/**
 * Write component/skill/score/icon fields from stage4 result back into
 * the original layoutJson object, then save it to disk at jsonPath.
 *
 * Strategy:
 *   1. Build a lookup map:  element_id -> mapped entry  (from result.mapped_components)
 *   2. Walk every element in layoutJson (sections[].elements[] or flat elements[])
 *   3. If the element's id is in the map, inject component/skill/score (and icon if present)
 *   4. Do the same for icon_elements using result.mapped_icon_elements
 *   5. Write the mutated layoutJson back to disk as pretty-printed JSON
 *
 * Fields written per element:
 *   component  {string}  - Syncfusion component name or "NATIVE_HTML"
 *   skill      {string|null} - Syncfusion skill package name
 *   score      {number}  - BM25 match confidence score
 *   icon       {object}  - { name, iconCss, score }  (only if icon_hint was present)
 *
 * @param {Object} layoutJson  - Original parsed component-mapping.json object
 * @param {Object} result      - Output of stage4ComponentPicking()
 * @param {string} jsonPath    - Absolute path to write the updated file
 */
function writeBackToJson(layoutJson, result, jsonPath) {
  // -- Build element_id -> mapped entry lookup --
  const componentLookup = {};
  for (const entry of (result.mapped_components || [])) {
    componentLookup[entry.element_id] = entry;
  }

  const iconLookup = {};
  for (const entry of (result.mapped_icon_elements || [])) {
    iconLookup[entry.element_id] = entry;
  }

  // -- Inject into sections[].elements[] --
  for (const section of (layoutJson.sections || [])) {
    for (const element of (section.elements || [])) {
      const mapped = componentLookup[element.id];
      if (mapped) {
        element.component = mapped.component;
        element.skill     = mapped.skill;
        element.score     = mapped.score;
        if (mapped.icon) {
          element.icon = mapped.icon;
        }
      }
    }
  }

  // -- Inject into flat elements[] (simple layout) --
  for (const element of (layoutJson.elements || [])) {
    const mapped = componentLookup[element.id];
    if (mapped) {
      element.component = mapped.component;
      element.skill     = mapped.skill;
      element.score     = mapped.score;
      if (mapped.icon) {
        element.icon = mapped.icon;
      }
    }
  }

  // -- Inject into icon_elements[] --
  for (const element of (layoutJson.icon_elements || [])) {
    const mapped = iconLookup[element.id];
    if (mapped && mapped.icon) {
      element.icon = mapped.icon;
    }
  }

  // -- Write updated JSON back to disk --
  fs.writeFileSync(jsonPath, JSON.stringify(layoutJson, null, 2), 'utf8');
}


// ---------------------------------------------------------------------------
// CLI entry point
// ---------------------------------------------------------------------------

if (require.main === module) {
  const args      = process.argv.slice(2);
  const scriptDir = __dirname;

  if (args.length > 0) {
    // Stage 4 mode: Process component-mapping.json file
    const jsonFile = args[0];

    let jsonPath = path.resolve(jsonFile);
    if (!path.isAbsolute(jsonFile)) {
      jsonPath = path.resolve(scriptDir, jsonFile);
    }

    const componentsCsv = path.join(scriptDir, 'components.csv');
    const iconsCsv      = path.join(scriptDir, 'icons.csv');

    try {
      if (!fs.existsSync(jsonPath)) {
        throw new Error(`JSON file not found: ${jsonFile}\nTried: ${jsonPath}`);
      }

      const layoutJson = JSON.parse(fs.readFileSync(jsonPath, 'utf8'));

      const result = stage4ComponentPicking(layoutJson, componentsCsv, iconsCsv);

      // 1. Log stage 4 output to stdout (existing behaviour)
      console.log(JSON.stringify(result, null, 2));

      // 2. Write component/skill/score/icon back into component-mapping.json
      writeBackToJson(layoutJson, result, jsonPath);

    } catch (err) {
      process.stderr.write(`Error: ${err.message}\n`);
      process.exit(1);
    }
  } else {
    process.stderr.write('Usage: node components-search.cjs <path-to-component-mapping.json>\n');
    process.exit(1);
  }
}
