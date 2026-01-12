import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  RefreshControl,
  SafeAreaView,
} from 'react-native';
import { getTermini, logout } from '../services/api';
import * as SecureStore from 'expo-secure-store';

export default function TerminiScreen({ navigation }) {
  const [termini, setTermini] = useState([]);
  const [loading, setLoading] = useState(false);
  const [userEmail, setUserEmail] = useState('');
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    loadUserData();
    loadTermini();
  }, []);

  const loadUserData = async () => {
    const email = await SecureStore.getItemAsync('userEmail');
    setUserEmail(email || '');
    setIsAdmin(email === 'admin@ebarber.si');
  };

  const loadTermini = async () => {
    try {
      setLoading(true);
      const data = await getTermini();
      setTermini(data);
    } catch (error) {
      alert('Napaka pri nalaganju terminov');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = async () => {
    await logout();
    navigation.replace('Login');
  };

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.headerText}>Moji Termini</Text>
        <Text style={styles.userEmail}>{userEmail}</Text>
        <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
          <Text style={styles.logoutText}>Odjava</Text>
        </TouchableOpacity>
      </View>

      {termini.length === 0 ? (
        <View style={styles.emptyContainer}>
          <Text style={styles.emptyText}>Nimate rezerviranih terminov</Text>
        </View>
      ) : (
        <FlatList
          data={termini}
          keyExtractor={(item) => item.id.toString()}
          refreshControl={
            <RefreshControl refreshing={loading} onRefresh={loadTermini} />
          }
          renderItem={({ item }) => (
            <View style={styles.terminCard}>
              <Text style={styles.terminDate}>
                {new Date(item.datumInUra).toLocaleString('sl-SI', {
                  weekday: 'long',
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric',
                  hour: '2-digit',
                  minute: '2-digit',
                })}
              </Text>
              <Text style={styles.terminLokacija}>
                📍 {item.lokacija.naslov}, {item.lokacija.mesto}
              </Text>
              <Text style={[
                styles.terminStatus,
                item.status === 'Čaka' && styles.statusCaka,
                item.status === 'Potrjen' && styles.statusPotrjen,
              ]}>
                Status: {item.status}
              </Text>
              {item.opombe && (
                <Text style={styles.opombe}>Opombe: {item.opombe}</Text>
              )}
            </View>
          )}
        />
      )}

      <View style={styles.fabContainer}>
        {isAdmin && (
          <TouchableOpacity
            style={[styles.fab, { backgroundColor: '#FF3B30' }]}
            onPress={() => navigation.navigate('AdminMenu')}
          >
            <Text style={styles.fabText}>🎛️</Text>
          </TouchableOpacity>
        )}

        <TouchableOpacity
          style={[styles.fab, styles.fabSecondary]}
          onPress={() => navigation.navigate('OkvirniTermini')}
        >
          <Text style={styles.fabText}>📅</Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.fab, { backgroundColor: '#FF9500' }]}
          onPress={() => navigation.navigate('Sporocila')}
        >
          <Text style={styles.fabText}>💬</Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.fab}
          onPress={() => navigation.navigate('NovTermin')}
        >
          <Text style={styles.fabText}>+</Text>
        </TouchableOpacity>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  header: {
    backgroundColor: '#007AFF',
    padding: 20,
    paddingTop: 10,
  },
  headerText: {
    fontSize: 24,
    fontWeight: 'bold',
    color: 'white',
    marginBottom: 5,
  },
  userEmail: {
    color: '#fff',
    fontSize: 14,
  },
  logoutButton: {
    backgroundColor: '#ff4444',
    padding: 10,
    borderRadius: 5,
    marginTop: 10,
  },
  logoutText: {
    color: 'white',
    textAlign: 'center',
    fontWeight: 'bold',
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 16,
    color: '#666',
  },
  terminCard: {
    backgroundColor: 'white',
    padding: 15,
    marginVertical: 8,
    marginHorizontal: 16,
    borderRadius: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  terminDate: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  terminLokacija: {
    fontSize: 14,
    color: '#666',
    marginBottom: 5,
  },
  terminStatus: {
    fontSize: 14,
    fontWeight: '600',
    marginTop: 5,
  },
  statusCaka: {
    color: '#FF9500',
  },
  statusPotrjen: {
    color: '#34C759',
  },
  opombe: {
    fontSize: 14,
    color: '#666',
    marginTop: 5,
    fontStyle: 'italic',
  },
  fabContainer: {
    position: 'absolute',
    right: 20,
    bottom: 20,
    flexDirection: 'column',
    gap: 12,
  },
  fab: {
    backgroundColor: '#007AFF',
    width: 60,
    height: 60,
    borderRadius: 30,
    justifyContent: 'center',
    alignItems: 'center',
    elevation: 4,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    marginBottom: 12,
  },
  fabSecondary: {
    backgroundColor: '#34C759',
  },
  fabText: {
    color: '#fff',
    fontSize: 30,
    fontWeight: 'bold',
  },
});