const cards = [...document.querySelectorAll('[data-service]')];
const controls = [...document.querySelectorAll('[data-controls]')];
let submitting = false;

function replaceEvents(card, events) {
    const body = card.querySelector('[data-events]');
    const rows = [...events].reverse().map(event => {
        const row = document.createElement('tr');
        const packet = event.type == null ? '—' : `0x${event.type.toString(16).toUpperCase().padStart(4, '0')}`;
        for (const value of [event.recordedAtUtc.slice(11, 19), event.event, event.session ?? '—', event.reason ?? event.state ?? packet]) {
            row.insertCell().textContent = value;
        }
        return row;
    });
    if (rows.length === 0) {
        const row = document.createElement('tr');
        const cell = row.insertCell();
        cell.colSpan = 4;
        cell.className = 'muted';
        cell.textContent = 'ยังไม่มีเหตุการณ์';
        rows.push(row);
    }
    body.replaceChildren(...rows);
}

function updateService(service) {
    const card = cards.find(item => item.dataset.service === service.id);
    const fields = {
        state: service.state, pid: service.pid ?? '—', uptime: service.uptime,
        connections: service.connections, memory: service.memoryMiB == null ? '—' : `${service.memoryMiB} MiB`,
        message: service.message ?? '', logSource: service.logSource
    };
    for (const [name, value] of Object.entries(fields)) card.querySelector(`[data-field="${name}"]`).textContent = value;
    card.querySelector('[data-field="state"]').className = `badge ${service.state.toLowerCase()}`;
    card.querySelector('[data-field="message"]').hidden = !service.message;
    card.dataset.connections = service.connections;
    replaceEvents(card, service.events);
}

async function refresh() {
    if (document.hidden || submitting) return;
    const monitor = document.querySelector('#monitor-state');
    try {
        const response = await fetch('/?handler=Monitor', { cache: 'no-store' });
        if (response.redirected) { location.assign('/SignIn'); return; }
        if (!response.ok) throw new Error('Monitor unavailable');
        const { services } = await response.json();
        if (submitting) return;
        for (const service of services) updateService(service);
        for (const form of controls) {
            const selection = services.filter(service => form.dataset.controls === 'all' || service.id === form.dataset.controls);
            for (const button of form.querySelectorAll('button')) {
                button.disabled = !selection.some(service => button.value === 'start' ? service.canStart : service.canStop);
            }
        }
        document.querySelector('#running').textContent = `${services.filter(service => service.state === 'Running').length} / ${services.length}`;
        document.querySelector('#connections').textContent = services.reduce((total, service) => total + service.connections, 0);
        monitor.textContent = 'ทุก 5 วินาที';
    } catch {
        monitor.textContent = 'กำลังเชื่อมต่อใหม่';
    }
}

for (const form of controls) {
    form.addEventListener('submit', event => {
        const action = event.submitter?.value;
        const active = cards.some(card => (form.dataset.controls === 'all' || form.dataset.controls === card.dataset.service)
            && Number(card.dataset.connections) > 0);
        if (active && (action === 'stop' || action === 'restart') && !window.confirm('บริการมีการเชื่อมต่ออยู่ ยืนยันตัดการเชื่อมต่อหรือไม่?')) {
            event.preventDefault();
            return;
        }
        submitting = true;
    });
}
window.setInterval(refresh, 5000);
