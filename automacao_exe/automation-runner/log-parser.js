function parseNUnitLogs(logText) {
  const logs = [];
  const lines = logText.split('\n');

  let currentTest = null;
  let inTestOutput = false;

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];

    // Parse test start
    if (line.includes('NUnit') || line.match(/Running.*tests?/i)) {
      logs.push({
        type: 'info',
        message: line.trim(),
        timestamp: new Date()
      });
    }

    // Parse test result
    const testResultMatch = line.match(/(Passed|Failed|Skipped):\s*(.+)/i);
    if (testResultMatch) {
      const [, status, testName] = testResultMatch;
      logs.push({
        type: status.toLowerCase() === 'passed' ? 'success' : 'error',
        message: line.trim(),
        testName: testName.trim(),
        timestamp: new Date()
      });
    }

    // Parse error messages
    if (line.match(/Error|Exception|Failed/i) && !line.includes('Passed')) {
      logs.push({
        type: 'error',
        message: line.trim(),
        timestamp: new Date()
      });
    }

    // Parse warnings
    if (line.match(/Warning|Warn/i)) {
      logs.push({
        type: 'warning',
        message: line.trim(),
        timestamp: new Date()
      });
    }

    // Parse Reqnroll output
    if (line.includes('Given') || line.includes('When') || line.includes('Then') || line.includes('And')) {
      logs.push({
        type: 'step',
        message: line.trim(),
        timestamp: new Date()
      });
    }

    // Parse test summary
    if (line.match(/Tests run|Total tests|Passed|Failed|Skipped/i)) {
      logs.push({
        type: 'summary',
        message: line.trim(),
        timestamp: new Date()
      });
    }
  }

  return logs;
}

function parseReqnrollOutput(logText) {
  const logs = [];
  const lines = logText.split('\n');

  for (const line of lines) {
    const trimmed = line.trim();
    if (!trimmed) continue;

    // Reqnroll step output
    if (trimmed.match(/^\s*(Given|When|Then|And|But)\s+/)) {
      logs.push({
        type: 'step',
        message: trimmed,
        timestamp: new Date()
      });
    }
    // Feature/Scenario info
    else if (trimmed.includes('Feature:') || trimmed.includes('Scenario:')) {
      logs.push({
        type: 'info',
        message: trimmed,
        timestamp: new Date()
      });
    }
    // Error messages
    else if (trimmed.match(/Error|Exception|Failed/i)) {
      logs.push({
        type: 'error',
        message: trimmed,
        timestamp: new Date()
      });
    }
    // Success indicators
    else if (trimmed.match(/Passed|Success|OK/i)) {
      logs.push({
        type: 'success',
        message: trimmed,
        timestamp: new Date()
      });
    }
    // Default to info
    else {
      logs.push({
        type: 'info',
        message: trimmed,
        timestamp: new Date()
      });
    }
  }

  return logs;
}

function extractTestSummary(logText) {
  const summary = {
    total: 0,
    passed: 0,
    failed: 0,
    skipped: 0,
    duration: 0
  };

  // NUnit summary patterns
  const totalMatch = logText.match(/Total tests:\s*(\d+)/i);
  const passedMatch = logText.match(/Passed:\s*(\d+)/i);
  const failedMatch = logText.match(/Failed:\s*(\d+)/i);
  const skippedMatch = logText.match(/Skipped:\s*(\d+)/i);
  const durationMatch = logText.match(/Duration:\s*([\d.]+)\s*ms/i);

  if (totalMatch) summary.total = parseInt(totalMatch[1]);
  if (passedMatch) summary.passed = parseInt(passedMatch[1]);
  if (failedMatch) summary.failed = parseInt(failedMatch[1]);
  if (skippedMatch) summary.skipped = parseInt(skippedMatch[1]);
  if (durationMatch) summary.duration = parseFloat(durationMatch[1]);

  return summary;
}

module.exports = {
  parseNUnitLogs,
  parseReqnrollOutput,
  extractTestSummary
};

