import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ScrollView,
  Alert,
} from 'react-native';
import { getLokacije, createTermin } from '../services/api';

export default function NovTerminScreen({ navigation }) {
  const [lokacije, setLokacije] = useState([]);
  const [selectedLokacija, setSelectedLokacija] = useState('');
  const [datum, setDatum] = useState('');
  const [ura, setUra] = useState('');
  const [opombe, setOpombe] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadLokacije();
  }, []);

  const loadLokacije = async () => {
    try {
      const data = await getLokacije();
      setLokacije(data);
      if (data.length > 0) {
        setSelectedLokacija(data[0].id.toString());
      }
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče naložiti lokacij');
    }
  };

  const handleSubmit = async () => {
    if (!datum || !ura || !selectedLokacija) {
      Alert.alert('Napaka', 'Prosim izpolni vse podatke');
      return;
    }

    const datumInUra = `${datum}T${ura}:00`;

    try {
      setLoading(true);
      await createTermin(datumInUra, parseInt(selectedLokacija), opombe);
      Alert.alert('Uspešno', 'Termin je bil uspešno rezerviran!', [
        { text: 'OK', onPress: () => navigation.goBack() },
      ]);
    } catch (error) {
      Alert.alert('Napaka', error.response?.data?.message || 'Ni mogoče ustvariti termina');
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Nova Rezervacija</Text>

      <Text style={styles.label}>Lokacija</Text>

      {lokacije.map((lok) => (
        <TouchableOpacity
          key={lok.id}
          style={[
            styles.locationItem,
            selectedLokacija === lok.id.toString() && styles.locationItemSelected,
          ]}
          onPress={() => setSelectedLokacija(lok.id.toString())}
        >
          <Text
            style={[
              styles.locationText,
              selectedLokacija === lok.id.toString() && styles.locationTextSelected,
            ]}
          >
            {lok.naslov}, {lok.mesto}
          </Text>
        </TouchableOpacity>
      ))}

      <Text style={styles.label}>Datum (YYYY-MM-DD)</Text>
      <TextInput
        style={styles.input}
        placeholder="2025-01-20"
        value={datum}
        onChangeText={setDatum}
      />

      <Text style={styles.label}>Ura (HH:MM)</Text>
      <TextInput
        style={styles.input}
        placeholder="14:00"
        value={ura}
        onChangeText={setUra}
      />

      <Text style={styles.label}>Opombe (opcijsko)</Text>
      <TextInput
        style={[styles.input, styles.textArea]}
        placeholder="Dodatne informacije..."
        value={opombe}
        onChangeText={setOpombe}
        multiline
        numberOfLines={4}
      />

      <TouchableOpacity
        style={[styles.button, loading && styles.buttonDisabled]}
        onPress={handleSubmit}
        disabled={loading}
      >
        <Text style={styles.buttonText}>
          {loading ? 'Rezerviram...' : 'Rezerviraj Termin'}
        </Text>
      </TouchableOpacity>

      <TouchableOpacity
        style={styles.cancelButton}
        onPress={() => navigation.goBack()}
      >
        <Text style={styles.cancelButtonText}>Prekliči</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  content: {
    padding: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
    marginTop: 20,
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 8,
    color: '#333',
  },
  input: {
    height: 50,
    borderColor: '#ddd',
    borderWidth: 1,
    borderRadius: 8,
    paddingHorizontal: 15,
    marginBottom: 20,
    fontSize: 16,
    backgroundColor: '#fff',
  },
  textArea: {
    height: 100,
    paddingTop: 15,
    textAlignVertical: 'top',
  },
  locationItem: {
    padding: 15,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
    marginBottom: 10,
    backgroundColor: '#fff',
  },
  locationItemSelected: {
    backgroundColor: '#007AFF',
    borderColor: '#007AFF',
  },
  locationText: {
    fontSize: 16,
    color: '#333',
  },
  locationTextSelected: {
    color: '#fff',
    fontWeight: 'bold',
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10,
  },
  buttonDisabled: {
    backgroundColor: '#ccc',
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
  cancelButton: {
    backgroundColor: '#fff',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  cancelButtonText: {
    color: '#666',
    fontSize: 16,
  },
});
