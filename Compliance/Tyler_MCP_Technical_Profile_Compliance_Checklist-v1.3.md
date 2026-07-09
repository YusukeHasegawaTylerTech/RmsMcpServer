# Tyler MCP Technical Profile Compliance Checklist - v1.3

*Based on: [Tyler MCP Technical Profile (Normative) - v1.3](https://tylertech.atlassian.net/wiki/spaces/TA1/pages/1130397801/Tyler+MCP+Technical+Profile+Normative+-+v1.3)*
*Last Updated: 2026-05-08*

---

## Instructions

This checklist is derived from the Tyler MCP Technical Profile (Normative) document. Each item preserves the original requirement language (MUST, SHOULD, MAY, SHALL NOT, etc.) from RFC 2119.

- **MUST / MUST NOT / SHALL NOT**: Mandatory requirements - all must be checked for compliance
- **SHOULD / SHOULD NOT**: Strongly recommended - document justification if not implemented
- **MAY**: Optional - implement based on your use case

**Submission Process:**
1. Rename this file to your MCP server instance name (e.g., `[PRODUCT]_MCP_Technical_Profile_Checklist-v1.3.md`)
2. Complete all applicable checkboxes throughout the document
3. Fill in the Compliance Summary section at the end with counts, reviewer information, and product details
4. Submit the completed checklist to the Foundry Team as a compliance artifact

Failure to comply with binding requirements prohibits designation of the MCP server as Tyler-certified or approved for external customer use.

**Companion Documents:**
- [Tyler MCP Security Overlay](https://tylertech.atlassian.net/wiki/x/OwTCMw) (enterprise security requirements)
- [Tyler MCP Implementation Guidance](https://tylertech.atlassian.net/wiki/x/94bUMw) (non-normative patterns and examples)

**MCP Specification Compliance:**
- [ ] MUST comply with the official MCP specification

---

## 1. Transport and Endpoint Conventions

### 1.1 Streamable HTTP Support

- [ ] MUST support Streamable HTTP transport
- [ ] All MCP endpoints MUST be under the `/mcp/*` path prefix (or the Tyler gateway-mapped equivalent) for security and governance management

**Implementation Notes:**
- Does your product expose multiple logical MCP servers? [ ] Yes [ ] No
- If yes, number of logical MCP servers: _____
- Endpoint URL(s):
  1. ___________________________
  2. ___________________________
  3. ___________________________
  4. ___________________________ (add more if needed)
- Gateway mapping (if applicable): ___________________________

**Examples of Compliant Patterns:**
- Single endpoint: `POST /mcp`
- Multiple endpoints: `POST /mcp/rev/gb/invoices`, `POST /mcp/eam/assets`

### 1.2 stdio Transport

- [ ] stdio transport is NOT implemented (recommended for production)
- [ ] stdio transport is implemented for local development only
- [ ] stdio is DISABLED in production deployments (MUST NOT be enabled in production)

**If stdio is implemented:**
- Development use only: [ ] Yes [ ] No
- Production disabled: [ ] Yes [ ] No

### 1.3 HTTP Protocol Support

- [ ] SHOULD support HTTP/2 where supported by the platform to improve multiplexing and latency

**If HTTP/2 not supported:**
- Justification: ___________________________
- Deployment platform constraints: ___________________________

---

## 2. Discovery and Interoperability

### 2.1 Tool Discovery

- [ ] MUST implement tool discovery in accordance with the MCP specification (`tools/list`)

**Verification:**
- `tools/list` endpoint implemented: [ ] Yes [ ] No
- Returns all available tools with metadata: [ ] Yes [ ] No

### 2.2 Resource Discovery (if resources are exposed)

**Does your MCP server expose resources?** [ ] Yes [ ] No

**If resources are exposed:**
- [ ] MUST implement resource discovery in accordance with the MCP specification (`resources/list`)
- [ ] MUST include required metadata fields per MCP specification

**Verification:**
- `resources/list` endpoint implemented: [ ] Yes [ ] No
- All required metadata fields present: [ ] Yes [ ] No

### 2.3 Prompt Exposure

- [ ] Prompts are NOT exposed (default recommended)
- [ ] Prompts ARE exposed (document justification below)

**If prompts are exposed:**
- Justification: ___________________________
- Type of prompts (user-facing actions, system prompts, etc.): ___________________________
- Security testing performed (e.g., promptfoo): [ ] Yes [ ] No

---

## 3. Tool Naming and Namespace Conventions

### 3.1 Namespace Prefix

- [ ] MUST use the Tyler namespace prefix: `tyl_*` for all tool names

**Verification:**
- All tools use `tyl_` prefix: [ ] Yes [ ] No
- If no, list non-compliant tools: ___________________________

### 3.2 Tool Naming Constraints

- [ ] MUST ensure tool names are ≤ 50 characters
- [ ] MUST ensure tool names contain only: `a-z`, `A-Z`, `0-9`, `_`, `-`

**Verification:**
- All tool names ≤ 50 characters: [ ] Yes [ ] No
- All tool names use allowed characters only: [ ] Yes [ ] No
- If non-compliant, list tools requiring remediation: ___________________________

### 3.3 Standard Action Verbs

- [ ] SHOULD map tool action components to standardized verbs: `get`, `create`, `update`, `delete`, `search`, `export`, `refresh`, `sync`, `calculate`, `submit`, `approve`, `void`

**If non-standard verbs are used:**
- List non-standard verbs and justification: ___________________________

---

## 4. Resource URI Conventions (if resources are exposed)

**Does your MCP server expose resources?** [ ] Yes [ ] No

**If resources are exposed:**

### 4.1 Tyler URI Scheme

- [ ] SHOULD support a Tyler custom scheme, for example: `tyl://{prod}/{module}/{resource}/{id}`

**If Tyler URI scheme is implemented:**
- URI scheme format: ___________________________
- Example URI: ___________________________

**If Tyler URI scheme is NOT implemented:**
- Justification: ___________________________
- Alternative URI scheme used: ___________________________

### 4.2 URI Templates

- [ ] SHOULD use RFC 6570-style URI templates where applicable

**If URI templates are used:**
- Compliant with RFC 6570: [ ] Yes [ ] No

**If URI templates are NOT used:**
- Justification: ___________________________

---

## 5. Session Management (Optional)

### 5.1 Stateful Sessions

**Does your MCP server use stateful sessions?** [ ] Yes [ ] No

**If stateful sessions are used:**
- [ ] MAY use MCP session features where stateful behavior is required
- [ ] If a session identifier is issued by the server, clients MUST present it on subsequent requests

**Session Implementation Details:**
- Session identifier mechanism: ___________________________
- Session storage (server-side, distributed, etc.): ___________________________
- Session lifetime: ___________________________
- Session revocation process: ___________________________

**If stateless:**
- [ ] MCP server is stateless (no session management required)

---

## 6. Gateway Routing (Tyler Deployment Convention)

### 6.1 External Exposure via Gateway

**Is your MCP server externally exposed?** [ ] Yes [ ] No

**If externally exposed:**
- [ ] MUST be routable through the Tyler MCP Gateway using approved routing conventions

**Gateway Routing Details:**
- Currently routed through Tyler MCP Gateway: [ ] Yes [ ] No (pending Gateway availability)
- If not yet routed through Gateway:
  - Corporate Security approval obtained: [ ] Yes [ ] No [ ] Pending
  - Gateway migration timeline: ___________________________

**If internal-only:**
- [ ] MCP server is internal-only (not externally exposed)

---

## Compliance Summary

**Product Information:**
- Product Name: ___________________________
- MCP Server Name: ___________________________
- GitHub Repository: ___________________________
- Deployment Environment: [ ] Development [ ] QA [ ] Production
- External Exposure: [ ] Yes [ ] No

**Compliance Counts:**
- Total MUST requirements applicable: _____ / _____
- Total MUST requirements compliant: _____ / _____
- Total SHOULD requirements applicable: _____ / _____
- Total SHOULD requirements compliant: _____ / _____
- Total exception requests: _____
- Total non-applicable items: _____

**Non-Compliant Items Summary:**
1. ___________________________
2. ___________________________
3. ___________________________

**Exception Requests:**
1. Requirement: ___________________________
   - Justification: ___________________________
   - Compensating controls: ___________________________
   - Remediation timeline (if temporary): ___________________________

**Reviewer Information:**
- Completed by (Name): ___________________________
- Role: ___________________________
- Date: ___________________________
- Contact: ___________________________

**Submission:**
- Submitted to Foundry Team: [ ] Yes [ ] No
- Submission Date: ___________________________

---

## Notes and Additional Context

___________________________
___________________________
___________________________
